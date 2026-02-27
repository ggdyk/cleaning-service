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