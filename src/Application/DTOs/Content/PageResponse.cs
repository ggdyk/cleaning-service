namespace Application.DTOs.Content;

/// <summary>
/// Локализованный ответ статической страницы.
/// Возвращает заголовок и контент на одном языке (определяется через ILanguageContext).
/// </summary>
public class PageResponse
{
    public string Slug { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string Content { get; init; } = default!;
    public DateTime UpdatedAt { get; init; }
}
