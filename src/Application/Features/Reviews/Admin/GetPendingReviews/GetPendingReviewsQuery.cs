using Application.DTOs.Reviews;
using MediatR;

namespace Application.Features.Reviews.Admin.GetPendingReviews;

public record GetPendingReviewsQuery : IRequest<IReadOnlyList<ReviewAdminResponse>>;
