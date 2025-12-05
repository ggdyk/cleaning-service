namespace Domain.Entities;

public class OrderService
{
    public Guid Id { get; set; } // айди записи услуги заказа
    public Guid OrderId { get; set; } // айди к которому относится услуга
    public Guid ServiceId { get; set; } // айди услуги
    public string ServiceName { get; set; } = default!; // название услуги, копируется при создании заказа
    public decimal Price { get; set; } // цена услуги на момент заказа
    public double Quantity { get; set; } // количество шт или квм
}