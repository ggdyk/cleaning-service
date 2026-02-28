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
- [ ] `Page`, `Review`, `CallBackRequest` — конфигурации и миграции
- [ ] CQRS + Controllers для Page, Review, CallBackRequest

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
| 2026-02-27 | AddCategories       | Таблица Categories + FK для Services|
| 2026-02-28 | AddCalculatorSettings | CalculatorSettings + ExtraServices |
| 2026-02-28 | AddOrderPriceBreakdown | 4 колонки разбивки цены в Orders  |

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
- Таблица `Services` отсутствует в БД — была потеряна при пересоздании базы.
  Колонки `name_kk`/`description_kk` из миграции `AddFAQs` будут применены при восстановлении.
- В Domain-сущностях `Page`, `CallBackRequest`, `Review`, `Payment` — CS8618 предупреждения
  (nullable свойства без инициализации). Существовали до текущей работы, не критично.

## Сделано сегодня (2026-02-28)

### Задача 3.9: Калькулятор цен (phase-3, priority-high) ✅
Полная вертикаль калькулятора:

- `CalculatorSettings` — доработан по стандарту (private set, Create/Update, BusinessRuleException)
- `ICalculatorSettingsRepository` + `CalculatorSettingsRepository` (GetByCityIdAsync, GetDefaultAsync)
- `IExtraServiceRepository` + `ExtraServiceRepository` (GetByIdsAsync, GetAllActiveAsync)
- `ExtraServiceConfiguration` — EF-конфигурация, таблица ExtraServices
- `CalculatorSettingsConfiguration` — EF-конфигурация, seed для города 1 (50₸/кв.м, 1000₸/санузел, мин 3000₸)
- Миграция `AddCalculatorSettings` (20260228) — применена
- DTOs: `CalculatePriceRequest`, `CalculatePriceResponse` (с детализацией: areaPrice, bathroomsPrice, servicePrice, extraServicesPrice, subtotal, total)
- CQRS: `CalculatePriceQuery` + `CalculatePriceHandler` + `CalculatePriceValidator`
- `CalculatorController` — `POST /api/calculator` (публичный)
- `CalculatorDefaultSettings` — Options pattern, fallback из appsettings.json
- `appsettings.json` — секция `Calculator` с дефолтными коэффициентами
- Сборка: **0 ошибок**

#### Формула расчёта
```
areaPrice      = area × PricePerSquareMeter
bathroomsPrice = bathrooms × PricePerBathroom
servicePrice   = service.BasePrice (если ServiceId указан)
extraServices  = sum(extraService.Price)
subtotal       = areaPrice + bathroomsPrice + servicePrice + extraServices
totalPrice     = max(subtotal, MinimumOrderAmount)
```

#### Приоритет настроек
1. Запись в БД `CalculatorSettings` для CityId → 2. Первая запись в БД (дефолт) → 3. appsettings.json

#### Endpoint
| Метод | URL | Доступ |
|-------|-----|--------|
| POST | /api/calculator | Публичный |

### Известные проблемы после реализации
- `Microsoft.Extensions.Options` и `Microsoft.Extensions.Configuration.Abstractions` добавлены в Application.csproj (необходимо для IOptions<T> в хендлере)

### Задача 3.10: Интегрировать калькулятор с заказом (phase-3, priority-medium) ✅

- `CreateOrderRequest` упрощён: убраны `ServiceName`, `UnitPrice`, `Name` — теперь клиент передаёт только IDs и количество
- `Order` entity: добавлены `AreaPrice`, `BathroomsPrice`, `ServicePrice`, `ExtraServicesPrice`
- `Order.Create()`: расширена сигнатура для приёма разбивки цены
- `CreateOrderHandler`: полный серверный расчёт (загружает Service/ExtraService из БД, коэффициенты из CalculatorSettings)
- `CreateOrderValidator`: удалены правила для убранных полей
- `OrderDetailsResponse` + `GetOrderByIdHandler`: разбивка цены в ответе
- `OrderConfiguration`: 4 новых decimal-колонки
- Миграция `AddOrderPriceBreakdown` — применена
- Сборка: **0 ошибок**

#### Безопасность
> Клиент больше **не может** подменить цену в запросе — все цены загружаются из БД на стороне сервера.

## Текущий фокус
> **Следующий шаг**: CQRS для Orders — `AssignCleaner` → `ChangeStatus` → `CancelOrder`
> либо продолжить Content — `Page`, `Review`, `CallBackRequest` (конфигурации + миграции + CQRS)