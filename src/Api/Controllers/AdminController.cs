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
    /// Получить настройки калькулятора для всех городов.
    /// </summary>
    [HttpGet("calculator-settings")]
    public async Task<ActionResult<IReadOnlyList<CalculatorSettingsResponse>>> GetCalculatorSettings()
    {
        var result = await _mediator.Send(new GetCalculatorSettingsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Создать или обновить настройки калькулятора для города.
    /// Если настройки для cityId уже существуют — обновляются, иначе создаются.
    /// Изменения применяются к новым расчётам немедленно.
    /// </summary>
    [HttpPut("calculator-settings")]
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
    /// Получить список заявок на обратный звонок.
    /// Опциональный фильтр: status (New / Processed / Rejected).
    /// </summary>
    [HttpGet("callbacks")]
    public async Task<ActionResult<IReadOnlyList<CallbackRequestResponse>>> GetAllCallbacks(
        [FromQuery] CallbackRequestStatus? status)
    {
        var result = await _mediator.Send(new GetAllCallbacksQuery(status));
        return Ok(result);
    }

    /// <summary>
    /// Сменить статус заявки (Processed или Rejected).
    /// </summary>
    [HttpPut("callbacks/{id:int}/status")]
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
    /// Получить все заказы с опциональными фильтрами.
    /// Параметры: status, dateFrom, dateTo, clientId.
    /// </summary>
    [HttpGet("orders")]
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
    /// Сменить статус заказа. Бизнес-правила переходов соблюдаются.
    /// Для перехода в Assigned требуется CleanerId в теле запроса.
    /// </summary>
    [HttpPut("orders/{id:int}/status")]
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
    /// Сводная статистика платформы: заказы, пользователи, заявки.
    /// </summary>
    [HttpGet("analytics/summary")]
    public async Task<ActionResult<AnalyticsSummaryDto>> GetAnalyticsSummary()
    {
        var result = await _mediator.Send(new GetAnalyticsSummaryQuery());
        return Ok(result);
    }

    /// <summary>
    /// Динамика заказов по периодам — данные для графика.
    /// groupBy: Day (по умолчанию), Week, Month.
    /// Пустые периоды возвращаются с count=0.
    /// По умолчанию: последние 30 дней, группировка по дням.
    /// </summary>
    [HttpGet("analytics/orders-by-period")]
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
    /// Топ популярных услуг по количеству заказов.
    /// Опциональные параметры: from, to — фильтр по периоду; top — количество позиций (по умолчанию 10).
    /// </summary>
    [HttpGet("analytics/popular-services")]
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
