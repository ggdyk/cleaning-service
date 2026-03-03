using Api.Authorization;
using Application.DTOs.Reviews;
using Application.Features.Reviews.Admin.ApproveReview;
using Application.Features.Reviews.Admin.GetAllReviews;
using Application.Features.Reviews.Admin.GetPendingReviews;
using Application.Features.Reviews.Admin.RejectReview;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

/// <summary>
/// Административный контроллер. Управление контентом платформы.
/// Все endpoints требуют роль Admin или Manager.
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize(Policy = Policies.AdminOrManager)]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // =========================================================================
    // Модерация отзывов
    // =========================================================================

    /// <summary>
    /// Получить все отзывы, включая неодобренные.
    /// </summary>
    [HttpGet("reviews")]
    public async Task<ActionResult<IReadOnlyList<ReviewAdminResponse>>> GetAllReviews()
    {
        var result = await _mediator.Send(new GetAllReviewsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Получить отзывы, ожидающие модерации (статус Pending).
    /// </summary>
    [HttpGet("reviews/pending")]
    public async Task<ActionResult<IReadOnlyList<ReviewAdminResponse>>> GetPendingReviews()
    {
        var result = await _mediator.Send(new GetPendingReviewsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Одобрить отзыв. Переводит статус Pending → Approved.
    /// </summary>
    [HttpPut("reviews/{id:int}/approve")]
    public async Task<ActionResult<ReviewAdminResponse>> ApproveReview(int id)
    {
        var moderatorId = GetCurrentUserId();
        var result = await _mediator.Send(new ApproveReviewCommand(id, moderatorId));
        return Ok(result);
    }

    /// <summary>
    /// Отклонить отзыв. Переводит статус Pending → Rejected.
    /// </summary>
    [HttpPut("reviews/{id:int}/reject")]
    public async Task<ActionResult<ReviewAdminResponse>> RejectReview(int id)
    {
        var moderatorId = GetCurrentUserId();
        var result = await _mediator.Send(new RejectReviewCommand(id, moderatorId));
        return Ok(result);
    }

    // -------------------------------------------------------------------------

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (claim == null || !int.TryParse(claim, out var userId))
            throw new UnauthorizedAccessException("Не удалось определить пользователя.");

        return userId;
    }
}
