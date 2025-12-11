using Domain.Common;

namespace Domain.Entities;

public class OrderService : BaseEntity
{
    public int OrderId { get; set; } // айди к которому относится услуга
    public int ServiceId { get; set; } // айди услуги
    public string ServiceName { get; set; } = default!; // название услуги, копируется при создании заказа
    public decimal Price { get; set; } // цена услуги на момент заказа
    public double Quantity { get; set; } // количество шт или квм
}