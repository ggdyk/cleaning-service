using Application.DTOs.Orders;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Orders.GetMyOrders;

public class GetMyOrdersHandler : IRequestHandler<GetMyOrdersQuery, IReadOnlyList<OrderSummaryResponse>>
{
    private readonly IOrderRepository _orderRepository;

    public GetMyOrdersHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<OrderSummaryResponse>> Handle(
        GetMyOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetByClientIdAsync(query.ClientId);

        return orders.Select(o => new OrderSummaryResponse
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            Status = o.Status.ToString(),
            TotalPrice = o.TotalPrice,
            CreatedAt = o.CreatedAt
        }).ToList();
    }
}
