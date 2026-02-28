namespace Application.DTOs.Calculator;

/// <summary>
/// Запрос на расчёт стоимости уборки.
/// </summary>
public class CalculatePriceRequest
{
    /// <summary>ID города — для выбора нужных коэффициентов.</summary>
    public int CityId { get; init; }

    /// <summary>Площадь помещения в кв.м.</summary>
    public double Area { get; init; }

    /// <summary>Количество санузлов.</summary>
    public int Bathrooms { get; init; }

    /// <summary>ID основной услуги (опционально).</summary>
    public int? ServiceId { get; init; }

    /// <summary>Список ID дополнительных услуг.</summary>
    public List<int> ExtraServiceIds { get; init; } = [];
}