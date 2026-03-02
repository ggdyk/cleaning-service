using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Статическая страница сайта с мультиязычным контентом (RU / KK / EN).
/// Идентифицируется по URL-friendly slug, например "about" или "contacts".
/// </summary>
public class Page : BaseEntity
{
    /// <summary>URL-friendly идентификатор страницы (например, "about", "contacts").</summary>
    public string Slug { get; private set; } = default!;

    /// <summary>Заголовок страницы на русском языке.</summary>
    public string TitleRu { get; private set; } = default!;

    /// <summary>Заголовок страницы на казахском языке.</summary>
    public string TitleKk { get; private set; } = default!;

    /// <summary>Заголовок страницы на английском языке.</summary>
    public string TitleEn { get; private set; } = default!;

    /// <summary>Содержимое страницы на русском языке.</summary>
    public string ContentRu { get; private set; } = default!;

    /// <summary>Содержимое страницы на казахском языке.</summary>
    public string ContentKk { get; private set; } = default!;

    /// <summary>Содержимое страницы на английском языке.</summary>
    public string ContentEn { get; private set; } = default!;

    /// <summary>Признак активности. Неактивные страницы не возвращаются клиентам.</summary>
    public bool IsActive { get; private set; }

    /// <summary>Дата и время последнего обновления (UTC).</summary>
    public DateTime UpdatedAt { get; private set; }

    // Конструктор для EF Core
    private Page() { }

    /// <summary>
    /// Создать новую страницу с мультиязычным контентом.
    /// </summary>
    public static Page Create(
        string slug,
        string titleRu,
        string titleKk,
        string titleEn,
        string contentRu,
        string contentKk,
        string contentEn)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Slug не может быть пустым.", nameof(slug));
        if (string.IsNullOrWhiteSpace(titleRu))
            throw new ArgumentException("Заголовок на русском не может быть пустым.", nameof(titleRu));
        if (string.IsNullOrWhiteSpace(contentRu))
            throw new ArgumentException("Содержимое на русском не может быть пустым.", nameof(contentRu));

        return new Page
        {
            Slug = slug.ToLowerInvariant().Trim(),
            TitleRu = titleRu.Trim(),
            TitleKk = (titleKk ?? string.Empty).Trim(),
            TitleEn = (titleEn ?? string.Empty).Trim(),
            ContentRu = contentRu.Trim(),
            ContentKk = (contentKk ?? string.Empty).Trim(),
            ContentEn = (contentEn ?? string.Empty).Trim(),
            IsActive = true,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Обновить содержимое страницы.
    /// </summary>
    public void Update(
        string titleRu,
        string titleKk,
        string titleEn,
        string contentRu,
        string contentKk,
        string contentEn)
    {
        if (string.IsNullOrWhiteSpace(titleRu))
            throw new ArgumentException("Заголовок на русском не может быть пустым.", nameof(titleRu));
        if (string.IsNullOrWhiteSpace(contentRu))
            throw new ArgumentException("Содержимое на русском не может быть пустым.", nameof(contentRu));

        TitleRu = titleRu.Trim();
        TitleKk = (titleKk ?? string.Empty).Trim();
        TitleEn = (titleEn ?? string.Empty).Trim();
        ContentRu = contentRu.Trim();
        ContentKk = (contentKk ?? string.Empty).Trim();
        ContentEn = (contentEn ?? string.Empty).Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Деактивировать страницу.</summary>
    public void Deactivate() => IsActive = false;

    /// <summary>Активировать страницу.</summary>
    public void Activate() => IsActive = true;
}
