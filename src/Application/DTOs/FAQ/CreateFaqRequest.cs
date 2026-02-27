namespace Application.DTOs.FAQ;

public class CreateFaqRequest
{
    public string QuestionRu { get; init; } = default!;
    public string QuestionKk { get; init; } = default!;
    public string QuestionEn { get; init; } = default!;
    public string AnswerRu { get; init; } = default!;
    public string AnswerKk { get; init; } = default!;
    public string AnswerEn { get; init; } = default!;
    public int SortOrder { get; init; }
}
