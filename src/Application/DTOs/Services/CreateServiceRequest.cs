namespace Application.DTOs.Services;

public class CreateServiceRequest
{
    public Guid CategoryId { get; init; }
    public string NameRu { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public string DescriptionRu { get; init; } = string.Empty;
    public string DescriptionEn { get; init; } = string.Empty;
    public decimal BasePrice { get; init; }
    public string Unit { get; init; } = string.Empty;
    public double? MinArea { get; init; }
    public int? DurationMinutes { get; init; }
    public int SortOrder { get; init; }
}