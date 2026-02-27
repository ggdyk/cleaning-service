using Application.DTOs.Orders;
using Application.Features.Orders.CreateOrder;
using Application.Features.Orders.GetMyOrders;
using Application.Features.Orders.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Создать новый заказ. Заказ привязывается к текущему авторизованному пользователю.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CreateOrderResponse>> Create([FromBody] CreateOrderRequest request)
    {
        var clientId = GetCurrentUserId();
        var command = new CreateOrderCommand(request, clientId);
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Получить список заказов текущего пользователя.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderSummaryResponse>>> GetMyOrders()
    {
        var clientId = GetCurrentUserId();
        var query = new GetMyOrdersQuery(clientId);
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Получить детали заказа. Доступно только владельцу заказа.
    /// </summary>
    [HttpGet("{id:int}")]
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
