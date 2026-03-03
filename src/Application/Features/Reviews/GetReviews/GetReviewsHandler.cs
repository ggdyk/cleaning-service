using Application.DTOs.Reviews;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Reviews.GetReviews;

public class GetReviewsHandler : IRequestHandler<GetReviewsQuery, IReadOnlyList<ReviewResponse>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetReviewsHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IReadOnlyList<ReviewResponse>> Handle(
        GetReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetApprovedAsync();

        return reviews
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewResponse
            {
                Id = r.Id,
                AuthorName = r.AuthorName,
                Rating = r.Rating,
                ReviewText = r.ReviewText,
                OrderId = r.OrderId,
                CreatedAt = r.CreatedAt
            })
            .ToList();
    }
}
