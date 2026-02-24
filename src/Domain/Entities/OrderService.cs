using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Снимок основной услуги в момент создания заказа.
/// Цена и название копируются из Catalog-контекста и фиксируются —
/// изменение цены в каталоге не влияет на уже созданные заказы.
/// </summary>
public class OrderService : BaseEntity
{
    /// <summary>ID заказа, к которому относится услуга.</summary>
    public int OrderId { get; private set; }

    /// <summary>ID услуги из Catalog контекста (только ссылка, не объект).</summary>
    public int ServiceId { get; private set; }

    /// <summary>Название услуги на момент создания заказа (снимок).</summary>
    public string ServiceName { get; private set; } = default!;

    /// <summary>Цена за единицу на момент создания заказа (снимок).</summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>Количество единиц (штуки или кв. метры в зависимости от услуги).</summary>
    public double Quantity { get; private set; }

    /// <summary>Итоговая стоимость этой услуги в заказе.</summary>
    public decimal TotalPrice => (decimal)Quantity * UnitPrice;

    // Конструктор для EF Core
    private OrderService() { }

    /// <summary>
    /// Создать снимок услуги для заказа.
    /// </summary>
    public static OrderService Create(
        int orderId,
        int serviceId,
        string serviceName,
        decimal unitPrice,
        double quantity)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
            throw new ArgumentException("Название услуги не может быть пустым.", nameof(serviceName));

        if (unitPrice < 0)
            throw new ArgumentException("Цена не может быть отрицательной.", nameof(unitPrice));

        if (quantity <= 0)
            throw new ArgumentException("Количество должно быть больше нуля.", nameof(quantity));

        return new OrderService
        {
            OrderId = orderId,
            ServiceId = serviceId,
            ServiceName = serviceName.Trim(),
            UnitPrice = unitPrice,
            Quantity = quantity
        };
    }
}