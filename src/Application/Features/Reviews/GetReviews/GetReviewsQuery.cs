using Application.DTOs.Reviews;
using MediatR;

namespace Application.Features.Reviews.GetReviews;

public record GetReviewsQuery : IRequest<IReadOnlyList<ReviewResponse>>;
