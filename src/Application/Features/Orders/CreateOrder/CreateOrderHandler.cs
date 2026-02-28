using Application.DTOs.Orders;
using Application.Features.Calculator.CalculatePrice;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Features.Orders.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IExtraServiceRepository _extraServiceRepository;
    private readonly ICalculatorSettingsRepository _settingsRepository;
    private readonly CalculatorDefaultSettings _defaults;

    public CreateOrderHandler(
        IOrderRepository orderRepository,
        IServiceRepository serviceRepository,
        IExtraServiceRepository extraServiceRepository,
        ICalculatorSettingsRepository settingsRepository,
        IOptions<CalculatorDefaultSettings> defaults)
    {
        _orderRepository = orderRepository;
        _serviceRepository = serviceRepository;
        _extraServiceRepository = extraServiceRepository;
        _settingsRepository = settingsRepository;
        _defaults = defaults.Value;
    }

    public async Task<CreateOrderResponse> Handle(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var req = command.Request;

        // 1. Загружаем настройки калькулятора для города
        var settings = await _settingsRepository.GetByCityIdAsync(req.CityId, cancellationToken)
                       ?? await _settingsRepository.GetDefaultAsync(cancellationToken);

        decimal pricePerSqm = settings?.PricePerSquareMeter ?? _defaults.PricePerSquareMeter;
        decimal pricePerBathroom = settings?.PricePerBathroom ?? _defaults.PricePerBathroom;
        decimal minimumOrderAmount = settings?.MinimumOrderAmount ?? _defaults.MinimumOrderAmount;

        // 2. Стоимость за площадь и санузлы
        var areaPrice = Math.Round((decimal)req.Area * pricePerSqm, 2);
        var bathroomsPrice = Math.Round(req.Bathrooms * pricePerBathroom, 2);

        // 3. Загружаем основные услуги из БД и считаем snapshot
        var serviceIds = req.Services.Select(s => s.ServiceId).ToList();
        var servicesFromDb = await LoadAndValidateServicesAsync(serviceIds, cancellationToken);

        decimal servicePrice = 0;
        var serviceItems = new List<(Domain.Entities.Service Service, double Quantity)>();
        foreach (var item in req.Services)
        {
            var svc = servicesFromDb[item.ServiceId];
            servicePrice += Math.Round(svc.BasePrice * (decimal)item.Quantity, 2);
            serviceItems.Add((svc, item.Quantity));
        }

        // 4. Загружаем доп. услуги из БД и считаем snapshot
        var extraIds = req.ExtraServices.Select(e => e.ExtraServiceId).ToList();
        var extrasFromDb = await LoadAndValidateExtraServicesAsync(extraIds, cancellationToken);

        decimal extraServicesPrice = 0;
        var extraItems = new List<(ExtraService Extra, int Quantity)>();
        foreach (var item in req.ExtraServices)
        {
            var ext = extrasFromDb[item.ExtraServiceId];
            extraServicesPrice += Math.Round(ext.Price * item.Quantity, 2);
            extraItems.Add((ext, item.Quantity));
        }

        // 5. Итоговая цена с учётом минимума
        var subtotal = areaPrice + bathroomsPrice + servicePrice + extraServicesPrice;
        var totalPrice = Math.Max(subtotal, minimumOrderAmount);

        // 6. Создаём заказ через фабричный метод Domain-сущности
        var order = Order.Create(
            clientId: command.ClientId,
            cityId: req.CityId,
            timeSlotId: req.TimeSlotId,
            street: req.Street,
            house: req.House,
            area: req.Area,
            bathrooms: req.Bathrooms,
            areaPrice: areaPrice,
            bathroomsPrice: bathroomsPrice,
            servicePrice: servicePrice,
            extraServicesPrice: extraServicesPrice,
            totalPrice: totalPrice,
            apartment: req.Apartment,
            entrance: req.Entrance,
            floor: req.Floor,
            doorCode: req.DoorCode,
            comment: req.Comment);

        // 7. Добавляем снимки услуг (имена и цены зафиксированы на момент создания)
        foreach (var (svc, qty) in serviceItems)
        {
            order.AddService(OrderService.Create(
                orderId: 0,
                serviceId: svc.Id,
                serviceName: svc.Name.Ru,
                unitPrice: svc.BasePrice,
                quantity: qty));
        }

        foreach (var (ext, qty) in extraItems)
        {
            order.AddExtraService(OrderExtraService.Create(
                orderId: 0,
                extraServiceId: ext.Id,
                name: ext.Name,
                unitPrice: ext.Price,
                quantity: qty));
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

    private async Task<Dictionary<int, Domain.Entities.Service>> LoadAndValidateServicesAsync(
        List<int> ids,
        CancellationToken ct)
    {
        var result = new Dictionary<int, Domain.Entities.Service>();
        foreach (var id in ids)
        {
            var svc = await _serviceRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException("Service", id);

            if (!svc.IsActive)
                throw new BusinessRuleException($"Услуга с ID={id} недоступна.");

            result[id] = svc;
        }
        return result;
    }

    private async Task<Dictionary<int, ExtraService>> LoadAndValidateExtraServicesAsync(
        List<int> ids,
        CancellationToken ct)
    {
        if (ids.Count == 0) return [];

        var extras = await _extraServiceRepository.GetByIdsAsync(ids, ct);
        var dict = extras.ToDictionary(e => e.Id);

        foreach (var id in ids)
        {
            if (!dict.TryGetValue(id, out var ext))
                throw new NotFoundException("ExtraService", id);

            if (!ext.IsActive)
                throw new BusinessRuleException($"Дополнительная услуга с ID={id} недоступна.");
        }

        return dict;
    }
}
