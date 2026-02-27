namespace Application.DTOs.Services;

/// <summary>
/// DTO для отображения услуги
/// </summary>
public class ServiceDto
{
    public int Id { get; init; }
    public Guid CategoryId { get; init; }
    public string NameRu { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public string DescriptionRu { get; init; } = String.Empty;
    public string DescriptionEn { get; init; } = String.Empty;
    public decimal BasePrice { get; init; }
    public string Unit { get; init; } = String.Empty;
    public double? MinArea { get; init; }
    public int? DurationMinutes { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
}