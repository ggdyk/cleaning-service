namespace Application.DTOs.Reviews;

public class ReviewAdminResponse
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string AuthorName { get; init; } = default!;
    public int Rating { get; init; }
    public string ReviewText { get; init; } = default!;
    public int? OrderId { get; init; }
    public string ModerationStatus { get; init; } = default!;
    public DateTime CreatedAt { get; init; }
    public DateTime? ModeratedAt { get; init; }
    public int? ModeratorId { get; init; }
}
