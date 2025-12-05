namespace Domain.Entities;

public class City
{
    public Guid Id { get; set; } // айди города
    public string Name { get; set; } = default!; 
    public bool IsActive { get; set; }
    public int SortOrder { get; set; } // порядок сортировки в интерфейсе
 
}