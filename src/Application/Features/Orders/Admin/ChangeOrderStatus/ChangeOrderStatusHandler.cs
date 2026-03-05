using Application.DTOs.Orders;
using Application.Interfaces;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Orders.Admin.ChangeOrderStatus;

public class ChangeOrderStatusHandler
    : IRequestHandler<ChangeOrderStatusCommand, AdminOrderSummaryResponse>
{
    private readonly IOrderRepository _orderRepository;

    public ChangeOrderStatusHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<AdminOrderSummaryResponse> Handle(
        ChangeOrderStatusCommand command,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId)
            ?? throw new NotFoundException("Заказ", command.OrderId);

        // Маппим запрошенный статус на соответствующий Domain-метод.
        // Бизнес-правила переходов (New → Assigned → InProgress → Completed,
        // New/Assigned → Cancelled) валидируются внутри методов Entity.
        switch (command.NewStatus)
        {
            case OrderStatus.Assigned:
                if (!command.CleanerId.HasValue)
                    throw new BusinessRuleException(
                        "Для перехода в статус Assigned необходимо указать CleanerId.");
                order.AssignCleaner(command.CleanerId.Value, command.AdminUserId);
                break;

            case OrderStatus.InProgress:
                order.StartWork(command.AdminUserId);
                break;

            case OrderStatus.Completed:
                order.Complete(command.AdminUserId);
                break;

            case OrderStatus.Cancelled:
                order.Cancel(command.AdminUserId, command.Comment);
                break;

            default:
                throw new BusinessRuleException(
                    $"Переход в статус '{command.NewStatus}' не поддерживается через этот endpoint.");
        }

        await _orderRepository.UpdateAsync(order);

        return new AdminOrderSummaryResponse
        {
            Id          = order.Id,
            OrderNumber = order.OrderNumber,
            Status      = order.Status.ToString(),
            TotalPrice  = order.TotalPrice,
            CreatedAt   = order.CreatedAt,
            ClientId    = order.ClientId,
            CleanerId   = order.CleanerId
        };
    }
}
