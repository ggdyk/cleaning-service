using Application.DTOs.Orders;
using Application.Features.Orders.CreateOrder;
using Application.Features.Orders.GetMyOrders;
using Application.Features.Orders.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

/// <summary>
/// Управление заказами клиента. Все endpoints требуют авторизации.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Создать новый заказ
    /// </summary>
    /// <remarks>
    /// Заказ привязывается к текущему авторизованному пользователю.
    /// Стоимость рассчитывается автоматически на основе выбранных услуг и параметров помещения.
    ///
    /// Пример запроса:
    ///
    ///     POST /api/orders
    ///     {
    ///         "cityId": 1,
    ///         "timeSlotId": 5,
    ///         "street": "ул. Абая",
    ///         "house": "10",
    ///         "apartment": "42",
    ///         "entrance": "2",
    ///         "floor": "7",
    ///         "area": 65.5,
    ///         "bathrooms": 1,
    ///         "comment": "Позвонить за 30 минут",
    ///         "services": [
    ///             { "serviceId": 1, "quantity": 1 }
    ///         ],
    ///         "extraServices": [
    ///             { "extraServiceId": 2, "quantity": 1 }
    ///         ]
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Заказ создан. Заголовок Location содержит URL нового ресурса</response>
    /// <response code="400">Ошибка валидации входных данных</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="404">Услуга, тайм-слот или город не найдены</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CreateOrderResponse>> Create([FromBody] CreateOrderRequest request)
    {
        var clientId = GetCurrentUserId();
        var command = new CreateOrderCommand(request, clientId);
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Получить список заказов текущего пользователя
    /// </summary>
    /// <remarks>
    /// Возвращает все заказы авторизованного клиента, отсортированные по дате создания (новые первые).
    /// </remarks>
    /// <response code="200">Список заказов (может быть пустым)</response>
    /// <response code="401">Не авторизован</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrderSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<OrderSummaryResponse>>> GetMyOrders()
    {
        var clientId = GetCurrentUserId();
        var query = new GetMyOrdersQuery(clientId);
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Получить детали заказа по ID
    /// </summary>
    /// <remarks>
    /// Доступно только владельцу заказа. Содержит полный состав услуг и историю статусов.
    /// </remarks>
    /// <param name="id">ID заказа</param>
    /// <response code="200">Детальная информация о заказе</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Заказ принадлежит другому пользователю</response>
    /// <response code="404">Заказ не найден</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDetailsResponse>> GetById(int id)
    {
        var clientId = GetCurrentUserId();
        var query = new GetOrderByIdQuery(id, clientId);
        var response = await _mediator.Send(query);
        return Ok(response);
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
