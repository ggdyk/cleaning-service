namespace Domain.Entities;

public class Category
{
    public Guid Id { get; set; } // айди категории услуг
    public string Name { get; set; } = default!; // название категории
    public string Description { get; set; } = default!; // описание категории
    public string IconUrl { get; set; } = default!; // изображение
    public int SortOrder { get; set; } // порядок отображения в списке
    public bool IsActive { get; set; } // признак активности категории
}