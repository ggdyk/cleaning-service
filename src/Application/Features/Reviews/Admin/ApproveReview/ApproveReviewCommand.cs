using Application.DTOs.Reviews;
using MediatR;

namespace Application.Features.Reviews.Admin.ApproveReview;

public record ApproveReviewCommand(int ReviewId, int ModeratorId) : IRequest<ReviewAdminResponse>;
