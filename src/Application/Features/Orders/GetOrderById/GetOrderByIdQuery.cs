using Application.DTOs.Orders;
using MediatR;

namespace Application.Features.Orders.GetOrderById;

public record GetOrderByIdQuery(int OrderId, int ClientId) : IRequest<OrderDetailsResponse>;
