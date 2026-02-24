using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

/// <summary>
/// Заказ на клининговую услугу — центральная сущность Core Domain.
/// Инкапсулирует весь жизненный цикл от создания до завершения.
/// </summary>
public class Order : BaseEntity
{
    /// <summary>Человекочитаемый номер заказа, например "ORD-20260224-0001".</summary>
    public string OrderNumber { get; private set; } = default!;

    /// <summary>ID клиента (из Identity контекста). Храним только ID, не объект User.</summary>
    public int ClientId { get; private set; }

    /// <summary>ID назначенного уборщика. Null до назначения.</summary>
    public int? CleanerId { get; private set; }

    /// <summary>ID города.</summary>
    public int CityId { get; private set; }

    /// <summary>ID временного слота.</summary>
    public int TimeSlotId { get; private set; }

    // --- Адрес ---
    public string Street { get; private set; } = default!;
    public string House { get; private set; } = default!;
    public string? Apartment { get; private set; }
    public string? Entrance { get; private set; }
    public string? Floor { get; private set; }
    public string? DoorCode { get; private set; }

    // --- Параметры помещения ---
    /// <summary>Площадь помещения в кв. метрах.</summary>
    public double Area { get; private set; }

    /// <summary>Количество санузлов.</summary>
    public int Bathrooms { get; private set; }

    /// <summary>Комментарий клиента к заказу.</summary>
    public string? Comment { get; private set; }

    // --- Цена и статус ---
    /// <summary>Итоговая цена заказа (фиксируется при создании).</summary>
    public decimal TotalPrice { get; private set; }

    /// <summary>Текущий статус заказа.</summary>
    public OrderStatus Status { get; private set; }

    // --- Временные метки ---
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // --- Связанные коллекции ---
    /// <summary>Список основных услуг заказа.</summary>
    public IReadOnlyList<OrderService> Services => _services.AsReadOnly();
    private readonly List<OrderService> _services = new();

    /// <summary>Список дополнительных услуг заказа.</summary>
    public IReadOnlyList<OrderExtraService> ExtraServices => _extraServices.AsReadOnly();
    private readonly List<OrderExtraService> _extraServices = new();

    /// <summary>История изменений статуса.</summary>
    public IReadOnlyList<OrderStatusHistory> StatusHistory => _statusHistory.AsReadOnly();
    private readonly List<OrderStatusHistory> _statusHistory = new();

    // Конструктор для EF Core (private — никто снаружи не должен его вызывать)
    private Order() { }

    /// <summary>
    /// Фабричный метод создания нового заказа.
    /// Бизнес-правило: новый заказ всегда получает статус New.
    /// </summary>
    public static Order Create(
        int clientId,
        int cityId,
        int timeSlotId,
        string street,
        string house,
        double area,
        int bathrooms,
        decimal totalPrice,
        string? apartment = null,
        string? entrance = null,
        string? floor = null,
        string? doorCode = null,
        string? comment = null)
    {
        if (clientId <= 0)
            throw new ArgumentException("ClientId должен быть положительным числом.", nameof(clientId));

        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Улица не может быть пустой.", nameof(street));

        if (string.IsNullOrWhiteSpace(house))
            throw new ArgumentException("Номер дома не может быть пустым.", nameof(house));

        if (area <= 0)
            throw new ArgumentException("Площадь должна быть больше нуля.", nameof(area));

        if (bathrooms < 0)
            throw new ArgumentException("Количество санузлов не может быть отрицательным.", nameof(bathrooms));

        if (totalPrice < 0)
            throw new ArgumentException("Цена не может быть отрицательной.", nameof(totalPrice));

        var now = DateTime.UtcNow;

        return new Order
        {
            OrderNumber = GenerateOrderNumber(now),
            ClientId = clientId,
            CityId = cityId,
            TimeSlotId = timeSlotId,
            Street = street.Trim(),
            House = house.Trim(),
            Apartment = apartment?.Trim(),
            Entrance = entrance?.Trim(),
            Floor = floor?.Trim(),
            DoorCode = doorCode?.Trim(),
            Area = area,
            Bathrooms = bathrooms,
            Comment = comment?.Trim(),
            TotalPrice = totalPrice,
            Status = OrderStatus.New,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    // -------------------------------------------------------------------------
    // Методы смены статуса (бизнес-логика)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Назначить уборщика на заказ.
    /// Допустимо только из статуса New.
    /// </summary>
    /// <param name="cleanerId">ID уборщика из Identity контекста.</param>
    /// <param name="changedByUserId">ID пользователя, который назначает (менеджер/admin).</param>
    public void AssignCleaner(int cleanerId, int changedByUserId)
    {
        if (Status != OrderStatus.New)
            throw new BusinessRuleException(
                $"Нельзя назначить уборщика — заказ в статусе '{Status}'. Ожидается: New.");

        if (cleanerId <= 0)
            throw new ArgumentException("CleanerId должен быть положительным числом.", nameof(cleanerId));

        var previous = Status;
        CleanerId = cleanerId;
        Status = OrderStatus.Assigned;
        UpdatedAt = DateTime.UtcNow;

        RecordStatusChange(previous, Status, changedByUserId);
    }

    /// <summary>
    /// Начать выполнение заказа.
    /// Допустимо только из статуса Assigned.
    /// </summary>
    /// <param name="changedByUserId">ID уборщика, который начинает работу.</param>
    public void StartWork(int changedByUserId)
    {
        if (Status != OrderStatus.Assigned)
            throw new BusinessRuleException(
                $"Нельзя начать работу — заказ в статусе '{Status}'. Ожидается: Assigned.");

        var previous = Status;
        Status = OrderStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;

        RecordStatusChange(previous, Status, changedByUserId);
    }

    /// <summary>
    /// Завершить заказ.
    /// Допустимо только из статуса InProgress.
    /// </summary>
    /// <param name="changedByUserId">ID уборщика, завершившего работу.</param>
    public void Complete(int changedByUserId)
    {
        if (Status != OrderStatus.InProgress)
            throw new BusinessRuleException(
                $"Нельзя завершить заказ — он в статусе '{Status}'. Ожидается: InProgress.");

        var previous = Status;
        Status = OrderStatus.Completed;
        UpdatedAt = DateTime.UtcNow;

        RecordStatusChange(previous, Status, changedByUserId);
    }

    /// <summary>
    /// Отменить заказ.
    /// Допустимо только из статусов New и Assigned.
    /// </summary>
    /// <param name="changedByUserId">ID пользователя, отменяющего заказ.</param>
    /// <param name="comment">Причина отмены (опционально).</param>
    public void Cancel(int changedByUserId, string? comment = null)
    {
        if (Status is not (OrderStatus.New or OrderStatus.Assigned))
            throw new BusinessRuleException(
                $"Нельзя отменить заказ — он в статусе '{Status}'. Отмена доступна из New или Assigned.");

        var previous = Status;
        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        RecordStatusChange(previous, Status, changedByUserId, comment);
    }

    // -------------------------------------------------------------------------
    // Вспомогательные методы
    // -------------------------------------------------------------------------

    /// <summary>Проверяет, можно ли отменить заказ (для клиента доступно только из New).</summary>
    public bool CanBeCancelledByClient() => Status == OrderStatus.New;

    /// <summary>Проверяет, завершён или отменён ли заказ.</summary>
    public bool IsClosed() => Status is OrderStatus.Completed or OrderStatus.Cancelled;

    private void RecordStatusChange(
        OrderStatus previous,
        OrderStatus next,
        int changedByUserId,
        string? comment = null)
    {
        _statusHistory.Add(OrderStatusHistory.Create(Id, previous, next, changedByUserId, comment));
    }

    private static string GenerateOrderNumber(DateTime createdAt)
    {
        // Формат: ORD-YYYYMMDD-XXXXXXXX (8 случайных символов для уникальности)
        var datePart = createdAt.ToString("yyyyMMdd");
        var uniquePart = Guid.NewGuid().ToString("N")[..8].ToUpper();
        return $"ORD-{datePart}-{uniquePart}";
    }
}