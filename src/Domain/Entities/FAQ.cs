namespace Domain.Entities;

public class FAQ
{
    public int Id { get; set; } // Уникальный идентификатор вопрос-ответ в системе;
    public string QuestionRu { get; set; } // Текст вопроса на русском языке;
    public string QuestionKk { get; set; } // Текст вопроса на казахском языке;
    public string QuestionEn { get; set; } // Текст вопроса на английский языке;
    public string AnswerRu { get; set; } // Текст ответа на русском языке
    public string AnswerKk { get; set; } // Текст ответа на казахском языке
    public string AnswerEn { get; set; } // Текст ответа на английском языке
    public int SortOrder { get; set; } // Порядок сортировки 
    public bool IsActive { get; set; } = true; // Признак активности
    public DateTime CreatedAt { get; set; } = DateTime.Now; // Дата и время создания записи FAQ
    public DateTime? UpdatedAt { get; set; } // Дата и время последнего обновления
}