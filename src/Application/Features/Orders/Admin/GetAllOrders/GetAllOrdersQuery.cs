using Application.DTOs.Orders;
using Domain.Enums;
using MediatR;

namespace Application.Features.Orders.Admin.GetAllOrders;

public record GetAllOrdersQuery(
    OrderStatus? Status,
    DateTime? DateFrom,
    DateTime? DateTo,
    int? ClientId
) : IRequest<IReadOnlyList<AdminOrderSummaryResponse>>;
