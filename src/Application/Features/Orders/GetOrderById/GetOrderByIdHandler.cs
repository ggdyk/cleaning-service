using Application.DTOs.Orders;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Orders.GetOrderById;

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailsResponse>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDetailsResponse> Handle(
        GetOrderByIdQuery query,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(query.OrderId)
            ?? throw new NotFoundException("Заказ", query.OrderId);

        if (order.ClientId != query.ClientId)
            throw new ForbiddenException("У вас нет доступа к этому заказу.");

        return new OrderDetailsResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            TotalPrice = order.TotalPrice,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Street = order.Street,
            House = order.House,
            Apartment = order.Apartment,
            Entrance = order.Entrance,
            Floor = order.Floor,
            DoorCode = order.DoorCode,
            Area = order.Area,
            Bathrooms = order.Bathrooms,
            Comment = order.Comment,
            AreaPrice = order.AreaPrice,
            BathroomsPrice = order.BathroomsPrice,
            ServicePrice = order.ServicePrice,
            ExtraServicesPrice = order.ExtraServicesPrice,
            CityId = order.CityId,
            TimeSlotId = order.TimeSlotId,
            CleanerId = order.CleanerId,
            Services = order.Services.Select(s => new OrderServiceResponse
            {
                ServiceId = s.ServiceId,
                ServiceName = s.ServiceName,
                UnitPrice = s.UnitPrice,
                Quantity = s.Quantity,
                TotalPrice = s.TotalPrice
            }).ToList(),
            ExtraServices = order.ExtraServices.Select(e => new OrderExtraServiceResponse
            {
                ExtraServiceId = e.ExtraServiceId,
                Name = e.Name,
                UnitPrice = e.UnitPrice,
                Quantity = e.Quantity,
                TotalPrice = e.TotalPrice
            }).ToList()
        };
    }
}
