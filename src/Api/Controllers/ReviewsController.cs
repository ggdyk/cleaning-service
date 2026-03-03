using Application.DTOs.Reviews;
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
