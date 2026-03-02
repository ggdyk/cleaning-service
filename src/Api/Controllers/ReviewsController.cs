using Api.Authorization;
using Application.DTOs.Reviews;
using Application.Features.Reviews.Admin.ApproveReview;
using Application.Features.Reviews.Admin.GetAllReviews;
using Application.Features.Reviews.Admin.GetPendingReviews;
using Application.Features.Reviews.Admin.RejectReview;
using Application.Features.Reviews.CreateReview;
using Application.Features.Reviews.GetReviews;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserRepository _userRepository;

    public ReviewsController(IMediator mediator, IUserRepository userRepository)
    {
        _mediator = mediator;
        _userRepository = userRepository;
    }

    // =========================================================================
    // Публичные endpoints
    // =========================================================================

    /// <summary>
    /// Получить список одобренных отзывов. Публичный доступ.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<ReviewResponse>>> GetApproved()
    {
        var result = await _mediator.Send(new GetReviewsQuery());
        return Ok(result);
    }

    // =========================================================================
    // Авторизованные endpoints (любой залогиненный пользователь)
    // =========================================================================

    /// <summary>
    /// Оставить отзыв. Требуется авторизация.
    /// Отзыв поступает на модерацию (статус Pending).
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReviewResponse>> Create([FromBody] CreateReviewRequest request)
    {
        var userId = GetCurrentUserId();
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        var command = new CreateReviewCommand(userId, user.GetFullName(), request);
        var result = await _mediator.Send(command);

        return StatusCode(201, result);
    }

    // =========================================================================
    // Админские endpoints (только Admin и Manager)
    // =========================================================================

    /// <summary>
    /// Получить все отзывы (любой статус). Только для Admin/Manager.
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<ActionResult<IReadOnlyList<ReviewAdminResponse>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllReviewsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Получить отзывы, ожидающие модерации. Только для Admin/Manager.
    /// </summary>
    [HttpGet("admin/pending")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<ActionResult<IReadOnlyList<ReviewAdminResponse>>> GetPending()
    {
        var result = await _mediator.Send(new GetPendingReviewsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Одобрить отзыв. Только для Admin/Manager.
    /// </summary>
    [HttpPut("admin/{id:int}/approve")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<ActionResult<ReviewAdminResponse>> Approve(int id)
    {
        var moderatorId = GetCurrentUserId();
        var result = await _mediator.Send(new ApproveReviewCommand(id, moderatorId));
        return Ok(result);
    }

    /// <summary>
    /// Отклонить отзыв. Только для Admin/Manager.
    /// </summary>
    [HttpPut("admin/{id:int}/reject")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<ActionResult<ReviewAdminResponse>> Reject(int id)
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
