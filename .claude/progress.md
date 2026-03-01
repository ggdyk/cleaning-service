# Project Progress

## Реализовано

### Identity (Generic Context) ✅ — образец для остальных контекстов
- [x] `User` entity с Value Objects (`Email`, `PasswordHash`)
- [x] `RefreshToken` entity
- [x] `UserRole` enum (Client, Cleaner, Manager, Admin)
- [x] `UserRepository`, `RefreshTokenRepository`
- [x] `IJwtService` / `JwtService` — генерация/валидация JWT
- [x] `IPasswordHasher` / `PasswordHasher` — BCrypt хеширование
- [x] CQRS: Register, Login, Logout, RefreshToken (Command + Handler + Validator)
- [x] `AuthController` — /api/auth/*
- [x] `ExceptionHandlingMiddleware`
- [x] EF конфигурация: `UserConfiguration`, `RefreshTokenConfiguration`

### Catalog (Supporting Context) ✅ — частично
- [x] Entities: `Service`, `ExtraService`, `Category`, `City`, `TimeSlot`, `CalculatorSettings`
- [x] EF конфигурация: `ServiceConfiguration`
- [ ] Репозитории и Features (CQRS)
- [ ] Controller

### Orders (Core Domain) 🔄 — в процессе
- [x] `Order` entity — полная бизнес-логика (Create, AssignCleaner, StartWork, Complete, Cancel)
- [x] `Order.AddService()`, `Order.AddExtraService()` — методы наполнения заказа
- [x] `OrderService`, `OrderExtraService`, `OrderStatusHistory` entities
- [x] `OrderStatus` enum (New → Assigned → InProgress → Completed | Cancelled)
- [x] EF конфигурации: Order, OrderService, OrderExtraService, OrderStatusHistory
- [x] Миграция `AddOrders` (20260224080026) — применена, таблицы в БД созданы
- [x] `IOrderRepository` + `OrderRepository` (GetByIdAsync с Include, GetByClientIdAsync, AddAsync)
- [x] CQRS: `CreateOrder` — Command + Handler + Validator
- [x] CQRS: `GetMyOrders` — Query + Handler (список заказов пользователя)
- [x] CQRS: `GetOrderById` — Query + Handler (детали, проверка владельца)
- [x] DTOs: `CreateOrderRequest/Response`, `OrderSummaryResponse`, `OrderDetailsResponse`
- [x] `OrdersController` — `POST /api/orders`, `GET /api/orders`, `GET /api/orders/{id}`
- [ ] CQRS Features: AssignCleaner, ChangeStatus, CancelOrder
- [ ] Endpoints для менеджера и уборщика

### Payment (Supporting Context) ❌ — не начат
- [ ] `Payment` entity (есть в Domain, без конфигурации)
- [ ] Mock-реализация PaymentService
- [ ] CQRS + Controller

### Notifications (Supporting Context) ❌ — не начат
- [ ] Email-уведомления по Order событиям
- [ ] Domain Events публикация из Orders

### Content (Supporting Context) 🔄 — в процессе
- [x] `FAQ` entity — переписан по стандарту (private set, Create, Update, Activate/Deactivate)
- [x] `LocalizedString` Value Object — добавлен казахский язык (Kk), перегрузка Create(ru, en)
- [x] `FAQConfiguration` — EF-конфигурация, индекс (IsActive, SortOrder)
- [x] Миграция `AddFAQs` (20260224193636) — таблица FAQs создана в БД
- [x] `IFAQRepository` + `FAQRepository` (GetActiveAsync, GetAllAsync, GetByIdAsync, Add, Update, Delete)
- [x] CQRS: `GetFAQs` — публичный список активных FAQ
- [x] CQRS: `GetAllFAQs` — все FAQ для Admin/Manager
- [x] CQRS: `CreateFAQ` — Command + Handler + Validator
- [x] CQRS: `UpdateFAQ` — Command + Handler + Validator
- [x] CQRS: `DeleteFAQ` — Command + Handler
- [x] `FAQController` — GET /api/faq (публичный), CRUD /api/faq/admin/* (Admin/Manager)
- [x] `ReviewModerationStatus` enum — Pending/Approved/Rejected
- [x] `Review` entity — переписан по стандарту (private set, Create, Approve, Reject, XML-комментарии)
- [x] `ReviewConfiguration` — EF-конфигурация, индексы (ModerationStatus, UserId, OrderId partial)
- [x] Миграция `AddReviews` (20260301134142) — таблица Reviews создана в БД
- [ ] `IReviewRepository` + `ReviewRepository` — CRUD
- [ ] CQRS + Controller для Review
- [ ] `Page`, `CallBackRequest` — конфигурации и миграции
- [ ] CQRS + Controllers для Page, CallBackRequest

### Admin (Generic Context) ❌ — не начат
- [ ] Статистика, управление пользователями
- [ ] Controller

## Миграции
| Дата       | Имя                 | Описание                            |
|------------|---------------------|-------------------------------------|
| 2025-12-30 | InitialWithServices | Начальная схема + сервисы           |
| 2026-02-23 | AddServices         | Seed-данные для Catalog             |
| 2026-02-24 | AddOrders           | Схема Orders                        |
| 2026-02-24 | AddFAQs             | Таблица FAQs + поля _kk для Services|
| 2026-02-27 | AddCategories       | Таблица Categories + FK к Services  |
| 2026-03-01 | AddReviews          | Таблица Reviews с модерацией        |

## Сделано сегодня (2026-03-01)

### Задача: Реализовать хранение отзывов клиентов (phase-4, backend, database, priority-medium)

**Выполнено:**
- `ReviewModerationStatus` enum — Pending=1, Approved=2, Rejected=3 (`Domain/Enums/`)
- `Review` entity — полностью переписан по стандарту: `private set`, `Create(userId, authorName, rating, reviewText, orderId?)`, `Approve(moderatorId)`, `Reject(moderatorId)`, XML-комментарии, бизнес-правила только в Domain
- `ReviewConfiguration` — EF: `AuthorName` varchar(200), `ReviewText` varchar(4000), enum→int, 3 индекса: `IX_Reviews_ModerationStatus`, `IX_Reviews_UserId`, `IX_Reviews_OrderId` (partial WHERE IS NOT NULL)
- `DbSet<Review> Reviews` — добавлен в `ApplicationDbContext`
- Миграция `AddReviews` (20260301134142) — применена, таблица `Reviews` в БД
- Сборка: **0 ошибок**

**Проблемы при реализации:**

1. **`dotnet ef database update` говорил "already up to date" при пустой истории** — оказалось, предыдущий запуск уже применил все миграции. Когда запустили откат до старой миграции (`database update <old>`) — EF интерпретировал это как "откатить всё выше" и дропнул `Reviews`. Потом применили через `migrations script` → psql напрямую.

2. **Миграция `AddCategories` падала на `DELETE FROM "Services"`** — таблица `Services` была потеряна при пересоздании БД ранее. Исправлено: все операции с `Services` в Up/Down обёрнуты в `DO $$ BEGIN IF EXISTS (...) THEN ... END IF; END $$;`.

## Сделано сегодня (2026-02-27)

### Задача: Создать endpoints для FAQ (phase-4, priority-medium)
Полная вертикаль от репозитория до контроллера:

- `IFAQRepository` — интерфейс в Application/Interfaces
- `FAQRepository` — реализация: GetActiveAsync, GetAllAsync, GetByIdAsync, Add, Update, Delete
- DTOs: `FaqResponse`, `CreateFaqRequest`, `UpdateFaqRequest`
- CQRS (5 features):
  - `GetFAQs` — публичный, только активные, сортировка по SortOrder
  - `GetAllFAQs` — все записи для Admin/Manager
  - `CreateFAQ` — с FluentValidation валидатором
  - `UpdateFAQ` — с FluentValidation валидатором
  - `DeleteFAQ`
- `FAQController` — 5 endpoints (см. таблицу ниже)
- Регистрация `IFAQRepository → FAQRepository` в Infrastructure/DependencyInjection.cs
- Сборка: **0 ошибок**

#### Endpoints FAQ
| Метод | URL | Доступ |
|-------|-----|--------|
| GET | /api/faq | Публичный |
| GET | /api/faq/admin | Admin, Manager |
| POST | /api/faq/admin | Admin, Manager |
| PUT | /api/faq/admin/{id} | Admin, Manager |
| DELETE | /api/faq/admin/{id} | Admin, Manager |

### Проблемы, возникшие при реализации

1. **Конфликт имён namespace vs тип** — папка `Features/FAQ/Admin/CreateFAQ` создала конфликт с `Domain.Entities.FAQ.Create`. Компилятор не мог разрешить `FAQ.Create(...)`. Решение: alias `using FaqEntity = Domain.Entities.FAQ;`

2. **Неверная сигнатура `NotFoundException`** — в хендлерах использовался `new NotFoundException("сообщение")`, но конструктор принимает `(string entityName, object entityId)`. Исправлено на `new NotFoundException("FAQ", command.Id)`.

## Известные проблемы
- **`dotnet ef database update` ненадёжен** — при нестандартном состоянии БД инструмент ведёт себя непредсказуемо. Надёжная альтернатива: `dotnet ef migrations script -o out.sql` → применить через psql напрямую.
- В Domain-сущностях `Page`, `CallBackRequest`, `Payment` — CS8618 предупреждения (nullable свойства без инициализации). Существовали до текущей работы, не критично.

## Текущий фокус
> **Следующий шаг**: `IReviewRepository` → `ReviewRepository` → CQRS Features → `ReviewsController`
> либо Orders: `AssignCleaner` → `ChangeStatus` → `CancelOrder`