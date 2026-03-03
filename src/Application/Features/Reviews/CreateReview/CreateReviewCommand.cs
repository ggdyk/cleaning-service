using Application.DTOs.Reviews;
using MediatR;

namespace Application.Features.Reviews.CreateReview;

public record CreateReviewCommand(
    int UserId,
    string AuthorName,
    CreateReviewRequest Request) : IRequest<ReviewResponse>;
