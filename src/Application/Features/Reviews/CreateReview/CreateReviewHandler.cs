using Application.DTOs.Reviews;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Reviews.CreateReview;

public class CreateReviewHandler : IRequestHandler<CreateReviewCommand, ReviewResponse>
{
    private readonly IReviewRepository _reviewRepository;

    public CreateReviewHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<ReviewResponse> Handle(
        CreateReviewCommand command,
        CancellationToken cancellationToken)
    {
        var review = Review.Create(
            userId: command.UserId,
            authorName: command.AuthorName,
            rating: command.Request.Rating,
            reviewText: command.Request.ReviewText,
            orderId: command.Request.OrderId);

        await _reviewRepository.AddAsync(review);

        return new ReviewResponse
        {
            Id = review.Id,
            AuthorName = review.AuthorName,
            Rating = review.Rating,
            ReviewText = review.ReviewText,
            OrderId = review.OrderId,
            CreatedAt = review.CreatedAt
        };
    }
}
