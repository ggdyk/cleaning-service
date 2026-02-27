using Application.DTOs.Orders;
using MediatR;

namespace Application.Features.Orders.CreateOrder;

public record CreateOrderCommand(CreateOrderRequest Request, int ClientId) : IRequest<CreateOrderResponse>;
