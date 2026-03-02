# Project Context

## Проект
**Cleaning Service Platform** — платформа для заказа клининговых услуг.

## Технический стек
| Компонент        | Технология                        |
|------------------|-----------------------------------|
| Runtime          | .NET 9.0 (SDK 9.0.306)            |
| Язык             | C# 13                             |
| БД               | PostgreSQL 16 (Docker, порт 5432) |
| ORM              | Entity Framework Core 9.0         |
| CQRS             | MediatR                           |
| Валидация        | FluentValidation                  |
| Auth             | JWT + Refresh Token               |
| API Docs         | Swashbuckle (Swagger)             |

## Архитектура
Clean Architecture + Domain-Driven Design + CQRS.

```
cleaning-service/
├── src/
│   ├── Api/                    # Controllers, Program.cs, Middleware, appsettings
│   ├── Application/            # Features (CQRS), DTOs, Interfaces
│   ├── Domain/                 # Entities, ValueObjects, Enums, Exceptions, Common
│   ├── Infrastructure/         # EF Core, Repositories, JWT, Migrations
│   └── Common/                 # Shared утилиты (если нужны)
├── docker-compose.yml          # PostgreSQL контейнер
├── global.json
└── cleaning-service.sln
```

## Bounded Contexts

| Контекст       | Тип          | Сущности                                              | Маршруты                           |
|----------------|--------------|-------------------------------------------------------|------------------------------------|
| Identity       | Generic      | User, RefreshToken                                    | /api/auth/*, /api/users/*          |
| Catalog        | Supporting   | Service, ServicePrice, ExtraService, ServiceCategory  | /api/services/*, /api/extra-services/* |
| Orders         | Core Domain  | Order, OrderService, OrderExtraService, OrderStatusHistory, TimeSlot | /api/orders/*     |
| Payment        | Supporting   | Payment (mock)                                        | /api/payments/*                    |
| Notifications  | Supporting   | Email по Order событиям                               | /api/notifications/*               |
| Content        | Supporting   | Page, FAQ, Review, BlogPost, CallbackRequest          | /api/pages/*, /api/reviews/*, /api/blog/*, /api/callback-requests/* |
| Admin          | Generic      | Статистика, управление пользователями                 | /api/admin/*                       |

## Роли пользователей
- **Client** — создаёт заказы, оставляет отзывы, отменяет New-заказы
- **Cleaner** — видит назначенные заказы, меняет статус InProgress/Completed
- **Manager** — назначает уборщиков, управляет контентом
- **Admin** — полный доступ

## База данных
- Контейнер: `cleaning_platform_db`
- БД: `cleaning_platform_dev`
- User: `postgres` / Password: `devpassword123`
- Каждый Bounded Context — своя схема PostgreSQL

## Ключевые интерфейсы
- `IUserRepository` — CRUD пользователей
- `IRefreshTokenRepository` — управление refresh-токенами
- `IJwtService` — генерация/валидация JWT
- `IPasswordHasher` — хеширование паролей

## Middleware
- `ExceptionHandlingMiddleware` — глобальная обработка исключений (NotFoundException, BusinessRuleException, ForbiddenException)

## Известные решения и подводные камни

### Конфликт имён: namespace Features vs тип Domain.Entities

**Проблема.** При создании CQRS-фичи для сущности, если namespace папки совпадает с именем Domain-типа, компилятор не может разрешить вызов статического метода.

Конкретный случай: папка `Application/Features/FAQ/Admin/CreateFAQ/` создаёт namespace `Application.Features.FAQ.Admin.CreateFAQ`. Внутри хендлера вызов `FAQ.Create(...)` становится неоднозначным — компилятор ищет `FAQ` в текущем namespace и находит папку `Features/FAQ`, а не `Domain.Entities.FAQ`.

**Решение.** Использовать alias в using:
```csharp
using FaqEntity = Domain.Entities.FAQ;

// затем в коде:
var faq = FaqEntity.Create(...);
```

**Правило.** Если имя Bounded Context совпадает с именем Entity (FAQ, Order, Payment и т.д.), и Feature находится во вложенном namespace этого контекста — всегда использовать alias для Domain-типа в хендлерах создания.

---

### Статус модерации отзыва — enum, а не строка

**Решение.** `ModerationStatus` в `Review` хранится как `enum ReviewModerationStatus` (Pending/Approved/Rejected), в БД — как `int`.

**Причина.** Исходный набросок `Review.cs` использовал `string ModerationStatus`. Это создаёт риск опечаток, затрудняет сравнение и не выражает допустимые переходы. Enum даёт типобезопасность на уровне компилятора, а бизнес-правила переходов (`Pending → Approved | Rejected`) закреплены прямо в методах `Approve()` / `Reject()` на Entity — в соответствии с архитектурным правилом "бизнес-логика только в Domain".

---

### Привязка отзыва к заказу — только через ID

**Решение.** `Review.OrderId` — `int?`, навигационного свойства нет, FK в БД не создаётся.

**Причина.** `Review` принадлежит контексту Content, `Order` — контексту Orders. По архитектурному правилу контексты общаются только через ID, не через объекты. Partial-индекс `WHERE "OrderId" IS NOT NULL` обеспечивает быструю выборку отзывов по заказу без нарушения границ контекстов.

---

### Применение миграций через `migrations script` + psql вместо `database update`

**Решение.** При нестандартном состоянии БД (несоответствие истории и реальных таблиц) использовать:
```bash
dotnet ef migrations script --project src/Infrastructure --startup-project src/Api -o out.sql
docker cp out.sql cleaning_platform_db:/tmp/out.sql
docker exec cleaning_platform_db psql -U postgres -d cleaning_platform_dev -f /tmp/out.sql
```

**Причина.** `dotnet ef database update` обнаружил проблему: при наличии миграций с ошибками в цепочке (миграция `AddCategories` падала на `DELETE FROM "Services"`, т.к. таблица была потеряна при пересоздании БД) инструмент откатывал всю транзакцию. Попытка откатить до конкретной старой миграции через `database update <name>` интерпретировалась EF как "откатить всё выше" — что дропнуло уже созданную таблицу `Reviews`. SQL-скрипт применяется напрямую без этих сложностей и даёт полный контроль над тем, что выполняется.

**Попутное исправление.** Миграция `AddCategories` была исправлена: все операции над таблицей `Services` в `Up()` и `Down()` обёрнуты в `DO $$ BEGIN IF EXISTS (...) THEN ... END IF; END $$;`. Теперь миграция идемпотентна и не падает если `Services` отсутствует.

---

### Именованные политики вместо `Roles = "..."` строк в атрибутах

**Решение.** Вместо `[Authorize(Roles = "Admin,Manager")]` — константы в `Api.Authorization.Policies` и `[Authorize(Policy = Policies.AdminOrManager)]`. Политики регистрируются один раз в `Program.cs` через `AddAuthorization(options => ...)`.

```csharp
// src/Api/Authorization/Policies.cs
public static class Policies
{
    public const string AdminOnly      = "AdminOnly";
    public const string AdminOrManager = "AdminOrManager";
    public const string CleanerOnly    = "CleanerOnly";
    public const string Staff          = "Staff";   // Admin + Manager + Cleaner
}

// Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.AdminOrManager, policy =>
        policy.RequireRole("Admin", "Manager"));
    // ...
});

// В контроллере:
[Authorize(Policy = Policies.AdminOrManager)]
```

**Причина.** Строковые роли в атрибутах (`Roles = "Admin,Manager"`) создают несколько проблем: опечатка в строке не поймается компилятором; логика "кто имеет доступ" размазана по всем контроллерам; при изменении состава роли нужно найти и поправить все места. Именованная политика — единая точка правды: изменение состава роли в одном месте (`Program.cs`) применяется везде автоматически. Дополнительный бонус: политики можно расширить до `RequireRole(...).RequireClaim(...)` без изменения контроллеров.

**Применение в проекте.** `FAQController` и `ReviewsController` переведены на политики. Все будущие контроллеры с ролевой защитой должны использовать `Policies.XYZ`, не строковые роли.

---

### Индекс на `User.Role` — для выборок уборщиков при назначении заказа

**Решение.** В `UserConfiguration` добавлен `HasIndex(x => x.Role).HasDatabaseName("IX_Users_Role")`. Миграция `AddUserRoleIndex` применена.

**Причина.** При реализации `AssignCleaner` (следующая задача по Orders) Manager видит список доступных уборщиков — запрос `WHERE Role = 'Cleaner'`. Без индекса это Full Table Scan по таблице Users. Добавить индекс проще заранее, чем после того как в системе появятся тысячи пользователей.