using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities;

/// <summary>
/// Дополнительная услуга из каталога (например, мойка окон, глажка, уборка холодильника).
/// Может быть добавлена к заказу. Цена фиксируется в момент оформления заказа (снимок).
/// </summary>
public class ExtraService : BaseEntity
{
    /// <summary>Название дополнительной услуги.</summary>
    public string Name { get; private set; } = default!;

    /// <summary>Описание услуги.</summary>
    public string Description { get; private set; } = default!;

    /// <summary>Цена за одну единицу.</summary>
    public decimal Price { get; private set; }

    /// <summary>Единица измерения (шт, кв.м и т.д.).</summary>
    public string Unit { get; private set; } = default!;

    /// <summary>Признак активности. Неактивные услуги недоступны для выбора.</summary>
    public bool IsActive { get; private set; }

    // Конструктор для EF Core
    private ExtraService() { }

    /// <summary>
    /// Создать новую дополнительную услугу.
    /// </summary>
    public static ExtraService Create(string name, string description, decimal price, string unit)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название не может быть пустым.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Описание не может быть пустым.", nameof(description));

        if (price < 0)
            throw new BusinessRuleException("Цена дополнительной услуги не может быть отрицательной.");

        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Единица измерения не может быть пустой.", nameof(unit));

        return new ExtraService
        {
            Name = name.Trim(),
            Description = description.Trim(),
            Price = price,
            Unit = unit.Trim(),
            IsActive = true
        };
    }

    /// <summary>
    /// Обновить данные дополнительной услуги.
    /// </summary>
    public void Update(string name, string description, decimal price, string unit, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название не может быть пустым.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Описание не может быть пустым.", nameof(description));

        if (price < 0)
            throw new BusinessRuleException("Цена дополнительной услуги не может быть отрицательной.");

        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Единица измерения не может быть пустой.", nameof(unit));

        Name = name.Trim();
        Description = description.Trim();
        Price = price;
        Unit = unit.Trim();
        IsActive = isActive;
    }
}
