using Application.DTOs.Orders;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Orders.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<CreateOrderResponse> Handle(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var req = command.Request;

        // Рассчитать итоговую цену из переданных услуг
        var totalPrice =
            req.Services.Sum(s => (decimal)s.Quantity * s.UnitPrice) +
            req.ExtraServices.Sum(e => e.Quantity * e.UnitPrice);

        // Создать заказ через фабричный метод Domain-сущности
        var order = Order.Create(
            clientId: command.ClientId,
            cityId: req.CityId,
            timeSlotId: req.TimeSlotId,
            street: req.Street,
            house: req.House,
            area: req.Area,
            bathrooms: req.Bathrooms,
            totalPrice: totalPrice,
            apartment: req.Apartment,
            entrance: req.Entrance,
            floor: req.Floor,
            doorCode: req.DoorCode,
            comment: req.Comment);

        // Добавить услуги (снимки на момент создания заказа)
        foreach (var s in req.Services)
        {
            order.AddService(OrderService.Create(
                orderId: 0, // EF присвоит Id после сохранения
                serviceId: s.ServiceId,
                serviceName: s.ServiceName,
                unitPrice: s.UnitPrice,
                quantity: s.Quantity));
        }

        // Добавить дополнительные услуги
        foreach (var e in req.ExtraServices)
        {
            order.AddExtraService(OrderExtraService.Create(
                orderId: 0,
                extraServiceId: e.ExtraServiceId,
                name: e.Name,
                unitPrice: e.UnitPrice,
                quantity: e.Quantity));
        }

        await _orderRepository.AddAsync(order);

        return new CreateOrderResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            TotalPrice = order.TotalPrice,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt
        };
    }
}
