using Application.DTOs.Reviews;
using MediatR;

namespace Application.Features.Reviews.Admin.RejectReview;

public record RejectReviewCommand(int ReviewId, int ModeratorId) : IRequest<ReviewAdminResponse>;
