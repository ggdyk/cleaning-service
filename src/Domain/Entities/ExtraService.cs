namespace Domain.Entities;

public class ExtraService
{
    public Guid Id { get; set; } // id доп услуги
    public string Name { get; set; } = default!; // название услуги
    public string Description { get; set; } = default!; // описание услуги
    public decimal Price { get; set; } // цена услуги
    public string Unit { get; set; } = default!; // ед измерения шт, квм 
    public bool IsActive { get; set; } // признак активности услуги
}