namespace Application.DTOs.Calculator;

/// <summary>
/// Результат расчёта стоимости уборки с детализацией по статьям.
/// </summary>
public class CalculatePriceResponse
{
    /// <summary>Стоимость за площадь (кв.м × коэффициент).</summary>
    public decimal AreaPrice { get; init; }

    /// <summary>Стоимость за санузлы (кол-во × коэффициент).</summary>
    public decimal BathroomsPrice { get; init; }

    /// <summary>Стоимость основной услуги (0 если не выбрана).</summary>
    public decimal ServicePrice { get; init; }

    /// <summary>Суммарная стоимость дополнительных услуг.</summary>
    public decimal ExtraServicesPrice { get; init; }

    /// <summary>Подытог до применения минимума.</summary>
    public decimal Subtotal { get; init; }

    /// <summary>Минимальная сумма заказа, применённая для данного города.</summary>
    public decimal MinimumOrderAmount { get; init; }

    /// <summary>Итоговая стоимость (не меньше MinimumOrderAmount).</summary>
    public decimal TotalPrice { get; init; }
}