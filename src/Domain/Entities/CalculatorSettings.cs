using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities;

/// <summary>
/// Настройки калькулятора цен для конкретного города.
/// Хранит коэффициенты расчёта стоимости уборки.
/// </summary>
public class CalculatorSettings : BaseEntity
{
    /// <summary>Идентификатор города, для которого действуют настройки.</summary>
    public int CityId { get; private set; }

    /// <summary>Цена за один квадратный метр площади.</summary>
    public decimal PricePerSquareMeter { get; private set; }

    /// <summary>Цена за один санузел.</summary>
    public decimal PricePerBathroom { get; private set; }

    /// <summary>Минимальная сумма заказа.</summary>
    public decimal MinimumOrderAmount { get; private set; }

    /// <summary>Дата последнего обновления настроек.</summary>
    public DateTime UpdatedAt { get; private set; }

    // Конструктор для EF Core
    private CalculatorSettings() { }

    /// <summary>
    /// Фабричный метод создания настроек калькулятора для города.
    /// </summary>
    /// <param name="cityId">ID города.</param>
    /// <param name="pricePerSquareMeter">Цена за кв.м (должна быть > 0).</param>
    /// <param name="pricePerBathroom">Цена за санузел (должна быть >= 0).</param>
    /// <param name="minimumOrderAmount">Минимальная сумма заказа (должна быть >= 0).</param>
    public static CalculatorSettings Create(
        int cityId,
        decimal pricePerSquareMeter,
        decimal pricePerBathroom,
        decimal minimumOrderAmount)
    {
        if (cityId <= 0)
            throw new BusinessRuleException("CityId должен быть положительным числом.");

        if (pricePerSquareMeter <= 0)
            throw new BusinessRuleException("Цена за кв.м должна быть больше нуля.");

        if (pricePerBathroom < 0)
            throw new BusinessRuleException("Цена за санузел не может быть отрицательной.");

        if (minimumOrderAmount < 0)
            throw new BusinessRuleException("Минимальная сумма заказа не может быть отрицательной.");

        return new CalculatorSettings
        {
            CityId = cityId,
            PricePerSquareMeter = pricePerSquareMeter,
            PricePerBathroom = pricePerBathroom,
            MinimumOrderAmount = minimumOrderAmount,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Обновляет коэффициенты расчёта.
    /// </summary>
    public void Update(
        decimal pricePerSquareMeter,
        decimal pricePerBathroom,
        decimal minimumOrderAmount)
    {
        if (pricePerSquareMeter <= 0)
            throw new BusinessRuleException("Цена за кв.м должна быть больше нуля.");

        if (pricePerBathroom < 0)
            throw new BusinessRuleException("Цена за санузел не может быть отрицательной.");

        if (minimumOrderAmount < 0)
            throw new BusinessRuleException("Минимальная сумма заказа не может быть отрицательной.");

        PricePerSquareMeter = pricePerSquareMeter;
        PricePerBathroom = pricePerBathroom;
        MinimumOrderAmount = minimumOrderAmount;
        UpdatedAt = DateTime.UtcNow;
    }
}