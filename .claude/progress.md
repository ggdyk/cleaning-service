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
- [x] `IOrderRepository` + `OrderRepository` (GetByIdAsync с Include, AddAsync)
- [x] CQRS: `CreateOrder` — Command + Handler + Validator
- [x] DTOs: `CreateOrderRequest`, `CreateOrderResponse`
- [x] `OrdersController` — `POST /api/orders` (требует JWT, ClientId из токена)
- [ ] CQRS Features: GetOrder, AssignCleaner, ChangeStatus, CancelOrder
- [ ] `GET /api/orders/{id}` и другие endpoints

### Payment (Supporting Context) ❌ — не начат
- [ ] `Payment` entity (есть в Domain, без конфигурации)
- [ ] Mock-реализация PaymentService
- [ ] CQRS + Controller

### Notifications (Supporting Context) ❌ — не начат
- [ ] Email-уведомления по Order событиям
- [ ] Domain Events публикация из Orders

### Content (Supporting Context) ❌ — не начат
- [ ] Entities: `Page`, `FAQ`, `Review`, `CallBackRequest` (есть в Domain)
- [ ] CQRS + Controllers

### Admin (Generic Context) ❌ — не начат
- [ ] Статистика, управление пользователями
- [ ] Controller

## Миграции
| Дата       | Имя              | Описание                      |
|------------|------------------|-------------------------------|
| 2025-12-30 | InitialWithServices | Начальная схема + сервисы   |
| 2026-02-23 | AddServices      | Дополнения к Catalog          |
| 2026-02-24 | AddOrders        | Схема Orders                  |

## Известные проблемы
- В Domain-сущностях `Page`, `FAQ`, `CallBackRequest`, `Review`, `Payment` — CS8618 предупреждения
  (nullable свойства без инициализации). Существовали до текущей работы, не критично.

## Текущий фокус
> **Следующий шаг**: Дореализовать оставшиеся CQRS-фичи Orders контекста:
> `GetOrder` → `AssignCleaner` → `ChangeStatus` → `CancelOrder` → endpoints в `OrdersController`