using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Запись в истории изменений статуса заказа.
/// Создаётся автоматически при каждой смене статуса через методы Order.
/// </summary>
public class OrderStatusHistory : BaseEntity
{
    /// <summary>ID заказа, к которому относится запись.</summary>
    public int OrderId { get; private set; }

    /// <summary>Статус до изменения.</summary>
    public OrderStatus PreviousStatus { get; private set; }

    /// <summary>Новый статус после изменения.</summary>
    public OrderStatus NewStatus { get; private set; }

    /// <summary>Дата и время изменения (UTC).</summary>
    public DateTime ChangedAt { get; private set; }

    /// <summary>ID пользователя, который изменил статус.</summary>
    public int ChangedByUserId { get; private set; }

    /// <summary>Комментарий к изменению (например, причина отмены).</summary>
    public string? Comment { get; private set; }

    // Конструктор для EF Core
    private OrderStatusHistory() { }

    /// <summary>
    /// Фабричный метод. Вызывается только внутри Order при смене статуса.
    /// </summary>
    internal static OrderStatusHistory Create(
        int orderId,
        OrderStatus previousStatus,
        OrderStatus newStatus,
        int changedByUserId,
        string? comment = null)
    {
        return new OrderStatusHistory
        {
            OrderId = orderId,
            PreviousStatus = previousStatus,
            NewStatus = newStatus,
            ChangedAt = DateTime.UtcNow,
            ChangedByUserId = changedByUserId,
            Comment = comment?.Trim()
        };
    }
}