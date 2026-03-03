namespace Application.DTOs.Reviews;

public class CreateReviewRequest
{
    public int Rating { get; init; }
    public string ReviewText { get; init; } = default!;
    public int? OrderId { get; init; }
}
