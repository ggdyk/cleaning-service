using Application.DTOs.Orders;
using Domain.Enums;
using MediatR;

namespace Application.Features.Orders.Admin.ChangeOrderStatus;

public record ChangeOrderStatusCommand(
    int OrderId,
    OrderStatus NewStatus,
    int AdminUserId,
    int? CleanerId,
    string? Comment
) : IRequest<AdminOrderSummaryResponse>;
