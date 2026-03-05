namespace Application.DTOs.CalculatorSettings;

/// <summary>
/// Запрос на создание или обновление настроек калькулятора для города.
/// Если настройки для CityId ещё не существуют — создаются новые.
/// </summary>
public class UpsertCalculatorSettingsRequest
{
    /// <summary>ID города, для которого задаются коэффициенты.</summary>
    public int CityId { get; init; }

    /// <summary>Цена за один квадратный метр (> 0).</summary>
    public decimal PricePerSquareMeter { get; init; }

    /// <summary>Цена за один санузел (>= 0).</summary>
    public decimal PricePerBathroom { get; init; }

    /// <summary>Минимальная сумма заказа (>= 0).</summary>
    public decimal MinimumOrderAmount { get; init; }
}
