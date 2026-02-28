namespace Application.Features.Calculator.CalculatePrice;

/// <summary>
/// Настройки калькулятора по умолчанию из appsettings.json.
/// Используются как fallback, если для города нет записи в БД.
/// </summary>
public class CalculatorDefaultSettings
{
    public const string SectionName = "Calculator";

    /// <summary>Цена за кв.м по умолчанию.</summary>
    public decimal PricePerSquareMeter { get; init; } = 50m;

    /// <summary>Цена за санузел по умолчанию.</summary>
    public decimal PricePerBathroom { get; init; } = 1000m;

    /// <summary>Минимальная сумма заказа по умолчанию.</summary>
    public decimal MinimumOrderAmount { get; init; } = 3000m;
}
