using Application.DTOs.Reviews;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Reviews.Admin.ApproveReview;

public class ApproveReviewHandler : IRequestHandler<ApproveReviewCommand, ReviewAdminResponse>
{
    private readonly IReviewRepository _reviewRepository;

    public ApproveReviewHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<ReviewAdminResponse> Handle(
        ApproveReviewCommand command,
        CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(command.ReviewId)
            ?? throw new NotFoundException("Review", command.ReviewId);

        review.Approve(command.ModeratorId);
        await _reviewRepository.UpdateAsync(review);

        return new ReviewAdminResponse
        {
            Id = review.Id,
            UserId = review.UserId,
            AuthorName = review.AuthorName,
            Rating = review.Rating,
            ReviewText = review.ReviewText,
            OrderId = review.OrderId,
            ModerationStatus = review.ModerationStatus.ToString(),
            CreatedAt = review.CreatedAt,
            ModeratedAt = review.ModeratedAt,
            ModeratorId = review.ModeratorId
        };
    }
}
