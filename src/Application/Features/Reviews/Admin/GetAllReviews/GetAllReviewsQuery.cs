using Application.DTOs.Reviews;
using MediatR;

namespace Application.Features.Reviews.Admin.GetAllReviews;

public record GetAllReviewsQuery : IRequest<IReadOnlyList<ReviewAdminResponse>>;
