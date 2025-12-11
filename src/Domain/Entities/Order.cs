using Domain.Common;

namespace Domain.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = default!; // человекачитаемый номер заказа
    public int UserId { get; set; } // айди клиента
    public int CityId { get; set; } 
    public int TimeSlotId { get; set; } // айди выбранного времени слота 

    public string Street { get; set; } = default!;
    public string House { get; set; } = default!;
    public string? Apartment { get; set; }
    public string? Entrance { get; set; }
    public string? Floor { get; set; }
    public string? DoorCode { get; set; }
    
    public double Area { get; set; } // площадь помещения
    public int Bathrooms { get; set; } // количество санузлов
    public string? Comment { get; set; } // комментарий клиента
    
    public decimal TotalPrice { get; set; } //  итоговая цена 
    public string Status { get; set; } = "new"; 
    
    public DateTime CreatedAt { get; set; } // дата создания 
    public DateTime UpdatedAt { get; set; } // дата последнего обновления 

    public List<OrderService> Services { get; set; } = new(); // список заказанных услуг
}