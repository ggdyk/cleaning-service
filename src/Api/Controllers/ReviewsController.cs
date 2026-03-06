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

/// <summary>
/// Отзывы клиентов о клининговом сервисе.
/// </summary>
[ApiController]
[Route("api/reviews")]
[Produces("application/json")]
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
    /// Получить список одобренных отзывов
    /// </summary>
    /// <remarks>
    /// Публичный endpoint — авторизация не требуется.
    /// Возвращает только отзывы со статусом Approved (прошедшие модерацию).
    /// </remarks>
    /// <response code="200">Список одобренных отзывов</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<ReviewResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ReviewResponse>>> GetApproved()
    {
        var result = await _mediator.Send(new GetReviewsQuery());
        return Ok(result);
    }

    // =========================================================================
    // Авторизованные endpoints (любой залогиненный пользователь)
    // =========================================================================

    /// <summary>
    /// Оставить отзыв
    /// </summary>
    /// <remarks>
    /// Требуется авторизация. Отзыв поступает на модерацию (статус Pending) —
    /// он не виден публично до одобрения администратором через `PUT /api/admin/reviews/{id}/approve`.
    ///
    /// Пример запроса:
    ///
    ///     POST /api/reviews
    ///     {
    ///         "rating": 5,
    ///         "reviewText": "Отличная уборка, всё чисто и аккуратно!",
    ///         "orderId": 42
    ///     }
    ///
    /// `orderId` — опциональное поле для привязки отзыва к конкретному заказу.
    /// </remarks>
    /// <response code="201">Отзыв принят на модерацию</response>
    /// <response code="400">Ошибка валидации (рейтинг вне диапазона 1–5, пустой текст)</response>
    /// <response code="401">Не авторизован</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
