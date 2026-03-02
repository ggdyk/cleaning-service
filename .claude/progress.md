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
- [x] `IReviewRepository` + `ReviewRepository` — GetApproved, GetPending, GetAll, GetById, Add, Update
- [x] DTOs: `ReviewResponse`, `ReviewAdminResponse`, `CreateReviewRequest`
- [x] CQRS: `GetReviews` — публичный, только Approved
- [x] CQRS: `CreateReview` — Command + Handler + Validator (FluentValidation)
- [x] CQRS: `GetAllReviews` — все отзывы для Admin/Manager
- [x] CQRS: `GetPendingReviews` — очередь модерации для Admin/Manager
- [x] CQRS: `ApproveReview` — Command + Handler (вызывает review.Approve())
- [x] CQRS: `RejectReview` — Command + Handler (вызывает review.Reject())
- [x] `ReviewsController` — 6 endpoints (см. таблицу ниже)
- [x] Регистрация `IReviewRepository → ReviewRepository` в Infrastructure/DependencyInjection.cs
- [x] Сборка: **0 ошибок**
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

## Сделано сегодня (2026-03-03)

### Задача: Разграничить права пользователей и админов (phase-5, auth, priority-high) ✅

**Выполнено:**
- `src/Api/Authorization/Policies.cs` — константы именованных политик: `AdminOnly`, `AdminOrManager`, `CleanerOnly`, `Staff`
- `Program.cs` — заменён `AddAuthorization()` на `AddAuthorization(options => ...)` с 4 зарегистрированными политиками
- `FAQController.cs`, `ReviewsController.cs` — `[Authorize(Roles = "Admin,Manager")]` заменено на `[Authorize(Policy = Policies.AdminOrManager)]`
- `TestController.cs` — переписан: 5 endpoints для ручной проверки всех политик через Swagger
- `UserConfiguration.cs` — добавлен `HasIndex(x => x.Role).HasDatabaseName("IX_Users_Role")`
- Миграция `AddUserRoleIndex` (20260302193048) — создана и применена в БД

**Что уже было готово до задачи:**
- `UserRole` enum в Domain (Client/Cleaner/Manager/Admin) — роли были с самого начала
- `User.Role` + `ChangeRole()` — в Entity
- JWT генерирует `ClaimTypes.Role` с `user.Role.ToString()` — работало с момента Auth реализации

**Проблемы при реализации:** не возникало — база была подготовлена правильно.

**Критерии готовности:**
- [x] Админские endpoints доступны только Admin/Manager → `[Authorize(Policy = Policies.AdminOrManager)]`
- [x] Обычные пользователи получают 403 → проверяется через `/api/test/admin-only`
- Сборка: **0 ошибок**

#### Endpoints TestController (для ручной проверки)
| URL | Политика | 200 | 403 |
|-----|----------|-----|-----|
| GET /api/test/public | — | Все | — |
| GET /api/test/me | `[Authorize]` | Любой залогиненный | Анонимный → 401 |
| GET /api/test/admin-only | `AdminOnly` | Admin | Client, Cleaner, Manager |
| GET /api/test/admin-or-manager | `AdminOrManager` | Admin, Manager | Client, Cleaner |
| GET /api/test/cleaner-only | `CleanerOnly` | Cleaner | Admin, Client, Manager |
| GET /api/test/staff | `Staff` | Admin, Manager, Cleaner | Client |

## Осталось сделать

### Приоритет 1 — Orders (Core Domain)
- [ ] CQRS: `AssignCleaner` — Command + Handler (Manager назначает уборщика на заказ)
- [ ] CQRS: `ChangeStatus` — Command + Handler (Cleaner: InProgress → Completed)
- [ ] CQRS: `CancelOrder` — Command + Handler (Client отменяет New-заказ)
- [ ] Endpoints в `OrdersController` для Manager и Cleaner

### Приоритет 2 — Content
- [ ] `Page`, `CallBackRequest` — EF-конфигурации, миграции, CQRS, Controllers

### Приоритет 3 — Payment, Notifications, Admin
- [ ] Payment: mock-реализация
- [ ] Notifications: Domain Events из Orders → email
- [ ] Admin: статистика, управление пользователями

## Сделано сегодня (2026-03-02)

### Задача: Создать endpoints для отзывов (phase-4, backend, priority-medium)

**Выполнено:**
- `IReviewRepository` — интерфейс: GetApprovedAsync, GetPendingAsync, GetAllAsync, GetByIdAsync, AddAsync, UpdateAsync
- `ReviewRepository` — реализация с фильтрацией по `ModerationStatus`
- DTOs: `ReviewResponse` (публичный), `ReviewAdminResponse` (с полями модерации), `CreateReviewRequest`
- CQRS (6 features):
  - `GetReviews` — публичный, только Approved, сортировка по убыванию даты
  - `CreateReview` — создаёт Review со статусом Pending, с FluentValidation валидатором
  - `GetAllReviews` — все отзывы для Admin/Manager
  - `GetPendingReviews` — только Pending, сортировка по возрастанию (FIFO модерация)
  - `ApproveReview` — вызывает `review.Approve(moderatorId)` через Domain метод
  - `RejectReview` — вызывает `review.Reject(moderatorId)` через Domain метод
- `ReviewsController` — 6 endpoints
- Регистрация DI в `Infrastructure/DependencyInjection.cs`
- Сборка: **0 ошибок**

#### Endpoints Reviews
| Метод | URL | Доступ |
|-------|-----|--------|
| GET | /api/reviews | Публичный |
| POST | /api/reviews | Авторизованный |
| GET | /api/reviews/admin | Admin, Manager |
| GET | /api/reviews/admin/pending | Admin, Manager |
| PUT | /api/reviews/admin/{id}/approve | Admin, Manager |
| PUT | /api/reviews/admin/{id}/reject | Admin, Manager |

**Архитектурные решения:**
- Имя автора берётся из `user.GetFullName()` через `IUserRepository` в контроллере — пользователь не может подменить имя
- `ApproveReview`/`RejectReview` не принимают тело запроса — всё управляется через URL и moderatorId из токена
- `InvalidOperationException` (повторная модерация) обрабатывается `ExceptionHandlingMiddleware`

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
> **Следующий шаг**: Orders: `AssignCleaner` → `ChangeStatus` → `CancelOrder`
> либо Content: `Page`, `CallBackRequest` — конфигурации, миграции, CQRS, Controllers