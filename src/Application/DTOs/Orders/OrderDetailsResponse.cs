namespace Application.DTOs.Orders;

/// <summary>
/// Полные детали заказа — для GET /api/orders/{id}.
/// </summary>
public class OrderDetailsResponse
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

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

    // Идентификаторы
    public int CityId { get; set; }
    public int TimeSlotId { get; set; }
    public int? CleanerId { get; set; }

    // Состав заказа
    public List<OrderServiceResponse> Services { get; set; } = new();
    public List<OrderExtraServiceResponse> ExtraServices { get; set; } = new();
}

public class OrderServiceResponse
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public double Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}

public class OrderExtraServiceResponse
{
    public int ExtraServiceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}
