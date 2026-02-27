using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Снимок дополнительной услуги в момент создания заказа.
/// Аналогично OrderService — цена фиксируется при создании заказа.
/// </summary>
public class OrderExtraService : BaseEntity
{
    /// <summary>ID заказа.</summary>
    public int OrderId { get; private set; }

    /// <summary>ID доп. услуги из Catalog контекста (только ссылка).</summary>
    public int ExtraServiceId { get; private set; }

    /// <summary>Название доп. услуги на момент создания заказа (снимок).</summary>
    public string Name { get; private set; } = default!;

    /// <summary>Цена за одну единицу доп. услуги (снимок).</summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>Количество единиц. Например, 2 мойки окон.</summary>
    public int Quantity { get; private set; }

    /// <summary>Итоговая стоимость этой доп. услуги.</summary>
    public decimal TotalPrice => UnitPrice * Quantity;

    /// <summary>Дата и время добавления в заказ (UTC).</summary>
    public DateTime CreatedAt { get; private set; }

    // Конструктор для EF Core
    private OrderExtraService() { }

    /// <summary>
    /// Создать снимок дополнительной услуги для заказа.
    /// </summary>
    public static OrderExtraService Create(
        int orderId,
        int extraServiceId,
        string name,
        decimal unitPrice,
        int quantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название доп. услуги не может быть пустым.", nameof(name));

        if (unitPrice < 0)
            throw new ArgumentException("Цена не может быть отрицательной.", nameof(unitPrice));

        if (quantity <= 0)
            throw new ArgumentException("Количество должно быть больше нуля.", nameof(quantity));

        return new OrderExtraService
        {
            OrderId = orderId,
            ExtraServiceId = extraServiceId,
            Name = name.Trim(),
            UnitPrice = unitPrice,
            Quantity = quantity,
            CreatedAt = DateTime.UtcNow
        };
    }
}