namespace Domain.Entities;

public class Service
{
    public Guid Id { get; set; } // айди услуги
    public Guid CategoryId { get; set; } // айди категории к которой принадлежит услуга
    public string Name { get; set; } = default!; // название услуги
    public string Description { get; set; } = default!; // описание услуги
    public decimal BasePrice { get; set; } // базовая цена услуги
    public string Unit { get; set; } = default!; // ед измерения (квм, услуга)
    public double? MinArea { get; set; } // мин площадь помещения для услуги
    public int? DurationMinutes { get; set; } // примерная длительность выполнения услуги
    public int SortOrder { get; set; } // порядок сортировки
    public bool IsActive { get; set; } // активность
}