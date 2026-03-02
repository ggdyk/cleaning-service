namespace Application.DTOs.FAQ;

/// <summary>
/// Локализованный ответ FAQ: возвращает вопрос/ответ на одном языке.
/// Используется публичным endpoint GET /api/faq.
/// Язык определяется через ILanguageContext (Accept-Language / ?lang=).
/// </summary>
public class FaqLocalizedResponse
{
    public int Id { get; init; }
    public string Question { get; init; } = default!;
    public string Answer { get; init; } = default!;
    public int SortOrder { get; init; }
}
