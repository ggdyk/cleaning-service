using Domain.Common;

namespace Domain.Entities;

public class City : BaseEntity
{
    public string Name { get; set; } = default!; 
    public bool IsActive { get; set; }
    public int SortOrder { get; set; } // порядок сортировки в интерфейсе
 
}