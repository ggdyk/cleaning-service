namespace Application.DTOs.Reviews;

public class ReviewResponse
{
    public int Id { get; init; }
    public string AuthorName { get; init; } = default!;
    public int Rating { get; init; }
    public string ReviewText { get; init; } = default!;
    public int? OrderId { get; init; }
    public DateTime CreatedAt { get; init; }
}
