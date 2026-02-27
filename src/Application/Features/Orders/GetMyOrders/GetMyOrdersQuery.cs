using Application.DTOs.Orders;
using MediatR;

namespace Application.Features.Orders.GetMyOrders;

public record GetMyOrdersQuery(int ClientId) : IRequest<IReadOnlyList<OrderSummaryResponse>>;
