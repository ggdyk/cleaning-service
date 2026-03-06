using Api.Authorization;
using Application.DTOs.Admin;
using Application.DTOs.CalculatorSettings;
using Application.DTOs.Callbacks;
using Application.DTOs.Orders;
using Application.DTOs.Reviews;
using Application.Features.Admin.GetAnalyticsSummary;
using Application.Features.Admin.GetOrdersByPeriod;
using Application.Features.Admin.GetPopularServices;
using Application.Features.Calculator.Admin.GetCalculatorSettings;
using Application.Features.Calculator.Admin.UpsertCalculatorSettings;
using Application.Features.Callbacks.Admin.ChangeCallbackStatus;
using Application.Features.Callbacks.Admin.GetAllCallbacks;
using Application.Features.Orders.Admin.ChangeOrderStatus;
using Application.Features.Orders.Admin.GetAllOrders;
using Application.Features.Reviews.Admin.ApproveReview;
using Application.Features.Reviews.Admin.GetAllReviews;
using Application.Features.Reviews.Admin.GetPendingReviews;
using Application.Features.Reviews.Admin.RejectReview;
using Domain.Enums;
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
[Produces("application/json")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // =========================================================================
    // Настройки калькулятора
    // =========================================================================

    /// <summary>
    /// Получить настройки калькулятора для всех городов
    /// </summary>
    /// <response code="200">Список настроек по городам</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав</response>
    [HttpGet("calculator-settings")]
    [ProducesResponseType(typeof(IReadOnlyList<CalculatorSettingsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<CalculatorSettingsResponse>>> GetCalculatorSettings()
    {
        var result = await _mediator.Send(new GetCalculatorSettingsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Создать или обновить настройки калькулятора для города
    /// </summary>
    /// <remarks>
    /// Если настройки для `cityId` уже существуют — обновляются, иначе создаются (upsert).
    /// Изменения применяются к новым расчётам немедленно.
    /// </remarks>
    /// <response code="200">Актуальные настройки после сохранения</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав</response>
    [HttpPut("calculator-settings")]
    [ProducesResponseType(typeof(CalculatorSettingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CalculatorSettingsResponse>> UpsertCalculatorSettings(
        [FromBody] UpsertCalculatorSettingsRequest request)
    {
        var result = await _mediator.Send(new UpsertCalculatorSettingsCommand(request));
        return Ok(result);
    }

    // =========================================================================
    // Управление заявками на звонок
    // =========================================================================

    /// <summary>
    /// Получить список заявок на обратный звонок
    /// </summary>
    /// <param name="status">Фильтр по статусу: New, Processed, Rejected (опционально)</param>
    /// <response code="200">Список заявок</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав</response>
    [HttpGet("callbacks")]
    [ProducesResponseType(typeof(IReadOnlyList<CallbackRequestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<CallbackRequestResponse>>> GetAllCallbacks(
        [FromQuery] CallbackRequestStatus? status)
    {
        var result = await _mediator.Send(new GetAllCallbacksQuery(status));
        return Ok(result);
    }

    /// <summary>
    /// Сменить статус заявки на звонок
    /// </summary>
    /// <param name="id">ID заявки</param>
    /// <param name="request">Новый статус заявки</param>
    /// <remarks>Допустимые переходы: New → Processed, New → Rejected.</remarks>
    /// <response code="200">Обновлённая заявка</response>
    /// <response code="400">Недопустимый переход статуса</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав</response>
    /// <response code="404">Заявка не найдена</response>
    [HttpPut("callbacks/{id:int}/status")]
    [ProducesResponseType(typeof(CallbackRequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CallbackRequestResponse>> ChangeCallbackStatus(
        int id,
        [FromBody] ChangeCallbackStatusRequest request)
    {
        var result = await _mediator.Send(
            new ChangeCallbackStatusCommand(id, request.Status));
        return Ok(result);
    }

    // =========================================================================
    // Управление заказами
    // =========================================================================

    /// <summary>
    /// Получить все заказы с опциональными фильтрами
    /// </summary>
    /// <param name="status">Фильтр по статусу (New, Assigned, InProgress, Completed, Cancelled)</param>
    /// <param name="dateFrom">Фильтр по дате создания — с (UTC)</param>
    /// <param name="dateTo">Фильтр по дате создания — по (UTC)</param>
    /// <param name="clientId">Фильтр по ID клиента</param>
    /// <response code="200">Список заказов</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав</response>
    [HttpGet("orders")]
    [ProducesResponseType(typeof(IReadOnlyList<AdminOrderSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<AdminOrderSummaryResponse>>> GetAllOrders(
        [FromQuery] OrderStatus? status,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] int? clientId)
    {
        var result = await _mediator.Send(
            new GetAllOrdersQuery(status, dateFrom, dateTo, clientId));
        return Ok(result);
    }

    /// <summary>
    /// Сменить статус заказа
    /// </summary>
    /// <param name="id">ID заказа</param>
    /// <param name="request">Новый статус и опциональные поля (cleanerId, comment)</param>
    /// <remarks>
    /// Бизнес-правила переходов статусов соблюдаются. Допустимые переходы:
    /// - New → Assigned (требуется `cleanerId`)
    /// - Assigned → InProgress
    /// - InProgress → Completed
    /// - New/Assigned → Cancelled
    /// </remarks>
    /// <response code="200">Обновлённый заказ</response>
    /// <response code="400">Недопустимый переход статуса</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав</response>
    /// <response code="404">Заказ или уборщик не найдены</response>
    [HttpPut("orders/{id:int}/status")]
    [ProducesResponseType(typeof(AdminOrderSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminOrderSummaryResponse>> ChangeOrderStatus(
        int id,
        [FromBody] ChangeOrderStatusRequest request)
    {
        var adminId = GetCurrentUserId();
        var result = await _mediator.Send(
            new ChangeOrderStatusCommand(id, request.Status, adminId, request.CleanerId, request.Comment));
        return Ok(result);
    }

    // =========================================================================
    // Аналитика
    // =========================================================================

    /// <summary>
    /// Сводная статистика платформы
    /// </summary>
    /// <remarks>Возвращает агрегаты: заказы по статусам, количество пользователей, необработанные заявки.</remarks>
    /// <response code="200">Сводная статистика</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав</response>
    [HttpGet("analytics/summary")]
    [ProducesResponseType(typeof(AnalyticsSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AnalyticsSummaryDto>> GetAnalyticsSummary()
    {
        var result = await _mediator.Send(new GetAnalyticsSummaryQuery());
        return Ok(result);
    }

    /// <summary>
    /// Динамика заказов по периодам — данные для графика
    /// </summary>
    /// <param name="startDate">Начало периода UTC (по умолчанию: 30 дней назад)</param>
    /// <param name="endDate">Конец периода UTC (по умолчанию: сейчас)</param>
    /// <param name="groupBy">Группировка: Day (по умолчанию), Week, Month</param>
    /// <remarks>Пустые периоды возвращаются с count = 0 — непрерывный ряд для графика.</remarks>
    /// <response code="200">Список точек данных для графика</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав</response>
    [HttpGet("analytics/orders-by-period")]
    [ProducesResponseType(typeof(IReadOnlyList<OrdersByPeriodItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<OrdersByPeriodItemDto>>> GetOrdersByPeriod(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] GroupByPeriod groupBy = GroupByPeriod.Day)
    {
        var from = startDate ?? DateTime.UtcNow.AddDays(-30);
        var to   = endDate   ?? DateTime.UtcNow;
        var result = await _mediator.Send(new GetOrdersByPeriodQuery(from, to, groupBy));
        return Ok(result);
    }

    /// <summary>
    /// Топ популярных услуг по количеству заказов
    /// </summary>
    /// <param name="from">Начало периода фильтрации (опционально)</param>
    /// <param name="to">Конец периода фильтрации (опционально)</param>
    /// <param name="top">Количество позиций в топе (по умолчанию 10)</param>
    /// <response code="200">Список услуг с количеством заказов</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав</response>
    [HttpGet("analytics/popular-services")]
    [ProducesResponseType(typeof(IReadOnlyList<PopularServiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<PopularServiceDto>>> GetPopularServices(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int top = 10)
    {
        var result = await _mediator.Send(new GetPopularServicesQuery(from, to, top));
        return Ok(result);
    }

    // =========================================================================
    // Модерация отзывов
    // =========================================================================

    /// <summary>
    /// Получить все отзывы, включая неодобренные
    /// </summary>
    /// <response code="200">Все отзывы со статусами модерации</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав</response>
    [HttpGet("reviews")]
    [ProducesResponseType(typeof(IReadOnlyList<ReviewAdminResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<ReviewAdminResponse>>> GetAllReviews()
    {
        var result = await _mediator.Send(new GetAllReviewsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Получить отзывы, ожидающие модерации (статус Pending)
    /// </summary>
    /// <response code="200">Список отзывов на модерации</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав</response>
    [HttpGet("reviews/pending")]
    [ProducesResponseType(typeof(IReadOnlyList<ReviewAdminResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<ReviewAdminResponse>>> GetPendingReviews()
    {
        var result = await _mediator.Send(new GetPendingReviewsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Одобрить отзыв — перевод статуса Pending → Approved
    /// </summary>
    /// <param name="id">ID отзыва</param>
    /// <response code="200">Одобренный отзыв</response>
    /// <response code="400">Отзыв уже одобрен или отклонён</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав</response>
    /// <response code="404">Отзыв не найден</response>
    [HttpPut("reviews/{id:int}/approve")]
    [ProducesResponseType(typeof(ReviewAdminResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewAdminResponse>> ApproveReview(int id)
    {
        var moderatorId = GetCurrentUserId();
        var result = await _mediator.Send(new ApproveReviewCommand(id, moderatorId));
        return Ok(result);
    }

    /// <summary>
    /// Отклонить отзыв — перевод статуса Pending → Rejected
    /// </summary>
    /// <param name="id">ID отзыва</param>
    /// <response code="200">Отклонённый отзыв</response>
    /// <response code="400">Отзыв уже одобрен или отклонён</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав</response>
    /// <response code="404">Отзыв не найден</response>
    [HttpPut("reviews/{id:int}/reject")]
    [ProducesResponseType(typeof(ReviewAdminResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
