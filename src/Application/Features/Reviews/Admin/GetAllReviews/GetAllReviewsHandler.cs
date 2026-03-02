using Application.DTOs.Reviews;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Reviews.Admin.GetAllReviews;

public class GetAllReviewsHandler : IRequestHandler<GetAllReviewsQuery, IReadOnlyList<ReviewAdminResponse>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetAllReviewsHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IReadOnlyList<ReviewAdminResponse>> Handle(
        GetAllReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetAllAsync();

        return reviews
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewAdminResponse
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
            })
            .ToList();
    }
}
