using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities;

/// <summary>
/// Категория услуг клининга
/// </summary>
public class Category : BaseEntity
{
    /// <summary>Мультиязычное название категории</summary>
    public LocalizedString Name { get; private set; }

    /// <summary>Мультиязычное описание категории</summary>
    public LocalizedString Description { get; private set; }

    /// <summary>URL иконки/изображения категории</summary>
    public string? IconUrl { get; private set; }

    /// <summary>Порядок сортировки</summary>
    public int SortOrder { get; private set; }

    /// <summary>Признак активности</summary>
    public bool IsActive { get; private set; }

    /// <summary>Услуги, относящиеся к категории</summary>
    public ICollection<Service> Services { get; private set; } = new List<Service>();

    // Конструктор для EF Core
    private Category()
    {
        Name = null!;
        Description = null!;
    }

    /// <summary>Фабричный метод создания категории</summary>
    public static Category Create(
        string nameRu,
        string nameKk,
        string nameEn,
        string descriptionRu,
        string descriptionKk,
        string descriptionEn,
        string? iconUrl,
        int sortOrder)
    {
        return new Category
        {
            Name = LocalizedString.Create(nameRu, nameKk, nameEn),
            Description = LocalizedString.Create(descriptionRu, descriptionKk, descriptionEn),
            IconUrl = iconUrl,
            SortOrder = sortOrder,
            IsActive = true
        };
    }

    /// <summary>Обновление полей категории</summary>
    public void Update(
        string nameRu,
        string nameKk,
        string nameEn,
        string descriptionRu,
        string descriptionKk,
        string descriptionEn,
        string? iconUrl,
        int sortOrder,
        bool isActive)
    {
        Name = LocalizedString.Create(nameRu, nameKk, nameEn);
        Description = LocalizedString.Create(descriptionRu, descriptionKk, descriptionEn);
        IconUrl = iconUrl;
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
