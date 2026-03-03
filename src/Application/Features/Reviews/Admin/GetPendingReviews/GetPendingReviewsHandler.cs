using Application.DTOs.Reviews;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Reviews.Admin.GetPendingReviews;

public class GetPendingReviewsHandler : IRequestHandler<GetPendingReviewsQuery, IReadOnlyList<ReviewAdminResponse>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetPendingReviewsHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IReadOnlyList<ReviewAdminResponse>> Handle(
        GetPendingReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetPendingAsync();

        return reviews
            .OrderBy(r => r.CreatedAt)
            .Select(MapToAdminResponse)
            .ToList();
    }

    private static ReviewAdminResponse MapToAdminResponse(Domain.Entities.Review r) => new()
    {
        Id = r.Id,
        UserId = r.UserId,
        AuthorName = r.AuthorName,
        Rating = r.Rating,
        ReviewText = r.ReviewText,
        OrderId = r.OrderId,
        ModerationStatus = r.ModerationStatus.ToString(),
        CreatedAt = r.CreatedAt,
        ModeratedAt = r.ModeratedAt,
        ModeratorId = r.ModeratorId
    };
}
