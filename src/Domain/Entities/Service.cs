using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities;

/// <summary>
/// Услуга клининга
/// </summary>
public class Service : BaseEntity
{
    /// <summary>
    /// Идентификатор категории услуги
    /// </summary>
    public Guid CategoryId { get; private set; }

    /// <summary>
    /// Мультиязычное название услуги
    /// </summary>
    public LocalizedString Name { get; private set; }

    /// <summary>
    /// Мультиязычное описание услуги
    /// </summary>
    public LocalizedString Description { get; private set; }

    /// <summary>
    /// Базовая цена услуги
    /// </summary>
    public decimal BasePrice { get; private set; }

    /// <summary>
    /// Единица измерения (кв.м, услуга и т.п.)
    /// </summary>
    public string Unit { get; private set; }

    /// <summary>
    /// Минимальная площадь (если применимо)
    /// </summary>
    public double? MinArea { get; private set; }

    /// <summary>
    /// Примерная длительность выполнения услуги (в минутах)
    /// </summary>
    public int? DurationMinutes { get; private set; }

    /// <summary>
    /// Порядок сортировки
    /// </summary>
    public int SortOrder { get; private set; }

    /// <summary>
    /// Признак активности услуги
    /// </summary>
    public bool IsActive { get; private set; }

    // Конструктор для EF Core
    private Service()
    {
        Name = null!;
        Description = null!;
        Unit = null!;
    }

    /// <summary>
    /// Фабричный метод создания услуги
    /// </summary>
    public static Service Create(
        Guid categoryId,
        string nameRu,
        string nameEn,
        string descriptionRu,
        string descriptionEn,
        decimal basePrice,
        string unit,
        double? minArea,
        int? durationMinutes,
        int sortOrder)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException("CategoryId is required", nameof(categoryId));

        if (basePrice <= 0)
            throw new ArgumentException("BasePrice must be greater than zero", nameof(basePrice));

        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Unit is required", nameof(unit));

        return new Service
        {
            CategoryId = categoryId,
            Name = LocalizedString.Create(nameRu, nameEn),
            Description = LocalizedString.Create(descriptionRu, descriptionEn),
            BasePrice = basePrice,
            Unit = unit.Trim(),
            MinArea = minArea,
            DurationMinutes = durationMinutes,
            SortOrder = sortOrder,
            IsActive = true
        };
    }

    /// <summary>
    /// Изменение цены услуги
    /// </summary>
    public void ChangePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(newPrice));

        BasePrice = newPrice;
    }

    /// <summary>
    /// Деактивация услуги
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Активация услуги
    /// </summary>
    public void Activate() => IsActive = true;
}