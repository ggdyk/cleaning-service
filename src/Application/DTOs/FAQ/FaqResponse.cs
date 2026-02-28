namespace Application.DTOs.FAQ;

/// <summary>
/// Ответ с данными одной записи FAQ.
/// Используется и в публичном списке, и в ответах админ-CRUD.
/// </summary>
public class FaqResponse
{
    public int Id { get; init; }
    public string QuestionRu { get; init; } = default!;
    public string QuestionKk { get; init; } = default!;
    public string QuestionEn { get; init; } = default!;
    public string AnswerRu { get; init; } = default!;
    public string AnswerKk { get; init; } = default!;
    public string AnswerEn { get; init; } = default!;
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
