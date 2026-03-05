using Application.DTOs.Orders;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Orders.Admin.GetAllOrders;

public class GetAllOrdersHandler
    : IRequestHandler<GetAllOrdersQuery, IReadOnlyList<AdminOrderSummaryResponse>>
{
    private readonly IOrderRepository _orderRepository;

    public GetAllOrdersHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<AdminOrderSummaryResponse>> Handle(
        GetAllOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllAsync(
            query.Status,
            query.DateFrom,
            query.DateTo,
            query.ClientId,
            cancellationToken);

        return orders.Select(o => new AdminOrderSummaryResponse
        {
            Id          = o.Id,
            OrderNumber = o.OrderNumber,
            Status      = o.Status.ToString(),
            TotalPrice  = o.TotalPrice,
            CreatedAt   = o.CreatedAt,
            ClientId    = o.ClientId,
            CleanerId   = o.CleanerId
        }).ToList();
    }
}
