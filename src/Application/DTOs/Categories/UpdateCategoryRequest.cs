namespace Application.DTOs.Categories;

public class UpdateCategoryRequest
{
    public string NameRu { get; init; } = string.Empty;
    public string NameKk { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public string DescriptionRu { get; init; } = string.Empty;
    public string DescriptionKk { get; init; } = string.Empty;
    public string DescriptionEn { get; init; } = string.Empty;
    public string? IconUrl { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
}
