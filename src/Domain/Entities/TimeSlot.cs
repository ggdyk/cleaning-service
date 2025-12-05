namespace Domain.Entities;

public class TimeSlot
{
    public Guid Id { get; set; } // айди слота времени
    public DateTime Date { get; set; } 
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
    public Guid CityId { get; set; }
    public int MaxOrders { get; set; } // макс допустимое количество заказов в слоте
    public int CurrentOrders { get; set; } // текущее количество заказов
    public bool IsAvailable { get; set; } // доступность для брони
}