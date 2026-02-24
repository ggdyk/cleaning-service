# Code Conventions

## Именование

### Файлы и папки
```
src/Application/Features/{Context}/{FeatureName}/
    {FeatureName}Command.cs      # или Query
    {FeatureName}Handler.cs
    {FeatureName}Validator.cs    # если нужна валидация
```

### C# классы
| Тип                     | Пример                            |
|-------------------------|-----------------------------------|
| Entity                  | `Order`, `User`                   |
| Value Object            | `Email`, `PasswordHash`           |
| Command                 | `CreateOrderCommand`              |
| Query                   | `GetOrderByIdQuery`               |
| Handler                 | `CreateOrderHandler`              |
| Repository Interface    | `IOrderRepository`                |
| Repository Impl         | `OrderRepository`                 |
| DTO (Response)          | `OrderResponse`, `OrderListItem`  |
| Controller              | `OrdersController`                |
| Exception               | `NotFoundException`, `BusinessRuleException` |

## Domain Layer — правила

### Entities
- Наследуются от `BaseEntity` (содержит `Id`)
- Все свойства `private set` — инкапсуляция
- Приватный конструктор для EF Core: `private Order() { }`
- Создание — через `static Order Create(...)` фабричный метод
- Бизнес-правила — методы на самой сущности (не в сервисах)
- Нарушение правила → `BusinessRuleException`
- Коллекции: `IReadOnlyList<T>` публично, `List<T>` приватно

### Value Objects
- `readonly record struct` или `readonly record`
- Валидация в конструкторе

### Исключения
| Класс                   | Когда бросать                         | HTTP статус |
|-------------------------|---------------------------------------|-------------|
| `NotFoundException`     | Сущность не найдена по ID             | 404         |
| `BusinessRuleException` | Нарушение бизнес-правил               | 422         |
| `ForbiddenException`    | Нет прав на операцию                  | 403         |

## Application Layer — правила

### CQRS (MediatR)
```csharp
// Command (изменение состояния)
public record CreateOrderCommand(...) : IRequest<OrderResponse>;

// Handler
public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
{
    public async Task<OrderResponse> Handle(CreateOrderCommand request, CancellationToken ct) { }
}

// Validator (FluentValidation)
public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator() { RuleFor(...) }
}
```

### Интерфейсы репозиториев — в Application/Interfaces
```csharp
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Order> AddAsync(Order order, CancellationToken ct = default);
    Task UpdateAsync(Order order, CancellationToken ct = default);
}
```

### DTOs
- Используются только в Application и Api слоях
- Domain сущности не возвращаются из Handlers напрямую
- Маппинг вручную (без AutoMapper по умолчанию)

## Infrastructure Layer — правила

### EF Core конфигурации
- Отдельный класс `{Entity}Configuration : IEntityTypeConfiguration<{Entity}>`
- Применяются в `ApplicationDbContext.OnModelCreating`
- Каждый Bounded Context — своя схема: `modelBuilder.HasDefaultSchema("orders")`

### Репозитории
- Реализуют интерфейс из Application/Interfaces
- Зависят только от `ApplicationDbContext`
- Асинхронные методы с `CancellationToken`

## API Layer — правила

### Controllers
```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    // ...
    [HttpPost]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
```

### HTTP статусы
- `200 OK` — успешный GET
- `201 Created` — успешный POST (новый ресурс)
- `204 No Content` — успешный DELETE/PATCH без тела
- `400 Bad Request` — ошибка валидации
- `401 Unauthorized` — не аутентифицирован
- `403 Forbidden` — нет прав
- `404 Not Found` — ресурс не найден
- `422 Unprocessable Entity` — нарушение бизнес-правил

## DI Регистрация
- `Application/DependencyInjection.cs` → `AddApplication()`
- `Infrastructure/DependencyInjection.cs` → `AddInfrastructure(IConfiguration)`
- Новые сервисы добавлять в соответствующий файл

## Общие принципы
- Контексты общаются только через внешние ID (не объекты)
- Бизнес-логика — только в Domain
- Application — оркестрация, не бизнес-правила
- Infrastructure — детали реализации (БД, HTTP, Email)
- Комментарии на русском (XML-doc для публичных API)