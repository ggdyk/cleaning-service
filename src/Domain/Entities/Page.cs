namespace Domain.Entities;

public class Page
{
    public int Id { get; set; } // Уникальный идентификатор страницы в системе
    public string Slug { get; set; } // Код страницы (URL-friendly идентификатор)
    public string TitleRu { get; set; } // Заголовок страницы на русском языке
    public string TitleKk { get; set; } // Заголовок страницы на казахском языке
    public string TitleEn { get; set; } // Заголовок страницы на английском языке
    public string ContentRu { get; set; } // Содержимое страницы на русском языке
    public string ContentKk { get; set; } // Содержимое страницы на казахском языке:
    public string ContentEn { get; set; } // Содержимое страницы на английском языке:
    public bool IsActive { get; set; } = true; // Признак активности (опубликована ли страница)
    public DateTime UpdatedAt { get; set; } = DateTime.Now; // Дата и время последнего обновления
}