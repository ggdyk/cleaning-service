namespace Application.DTOs.Orders;

public class CreateOrderRequest
{
    public int CityId { get; set; }
    public int TimeSlotId { get; set; }

    // Адрес
    public string Street { get; set; } = string.Empty;
    public string House { get; set; } = string.Empty;
    public string? Apartment { get; set; }
    public string? Entrance { get; set; }
    public string? Floor { get; set; }
    public string? DoorCode { get; set; }

    // Параметры помещения
    public double Area { get; set; }
    public int Bathrooms { get; set; }

    public string? Comment { get; set; }

    // Услуги
    public List<OrderServiceItem> Services { get; set; } = new();
    public List<OrderExtraServiceItem> ExtraServices { get; set; } = new();
}

public class OrderServiceItem
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public double Quantity { get; set; }
}

public class OrderExtraServiceItem
{
    public int ExtraServiceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
