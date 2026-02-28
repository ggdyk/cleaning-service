using Application.DTOs.Calculator;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Features.Calculator.CalculatePrice;

public class CalculatePriceHandler : IRequestHandler<CalculatePriceQuery, CalculatePriceResponse>
{
    private readonly ICalculatorSettingsRepository _settingsRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IExtraServiceRepository _extraServiceRepository;
    private readonly CalculatorDefaultSettings _defaults;

    public CalculatePriceHandler(
        ICalculatorSettingsRepository settingsRepository,
        IServiceRepository serviceRepository,
        IExtraServiceRepository extraServiceRepository,
        IOptions<CalculatorDefaultSettings> defaults)
    {
        _settingsRepository = settingsRepository;
        _serviceRepository = serviceRepository;
        _extraServiceRepository = extraServiceRepository;
        _defaults = defaults.Value;
    }

    public async Task<CalculatePriceResponse> Handle(
        CalculatePriceQuery request,
        CancellationToken cancellationToken)
    {
        var req = request.Request;

        // 1. Загружаем настройки для города, fallback → дефолт из БД → дефолт из конфига
        var settings = await _settingsRepository.GetByCityIdAsync(req.CityId, cancellationToken)
                       ?? await _settingsRepository.GetDefaultAsync(cancellationToken);

        decimal pricePerSqm = settings?.PricePerSquareMeter ?? _defaults.PricePerSquareMeter;
        decimal pricePerBathroom = settings?.PricePerBathroom ?? _defaults.PricePerBathroom;
        decimal minimumOrderAmount = settings?.MinimumOrderAmount ?? _defaults.MinimumOrderAmount;

        // 2. Стоимость за площадь и санузлы
        var areaPrice = (decimal)req.Area * pricePerSqm;
        var bathroomsPrice = req.Bathrooms * pricePerBathroom;

        // 3. Стоимость основной услуги
        decimal servicePrice = 0;
        if (req.ServiceId.HasValue)
        {
            var service = await _serviceRepository.GetByIdAsync(req.ServiceId.Value, cancellationToken);
            if (service is null)
                throw new NotFoundException("Service", req.ServiceId.Value);
            if (!service.IsActive)
                throw new BusinessRuleException("Выбранная услуга недоступна.");

            servicePrice = service.BasePrice;
        }

        // 4. Стоимость дополнительных услуг
        decimal extraServicesPrice = 0;
        if (req.ExtraServiceIds.Count > 0)
        {
            var extraServices = await _extraServiceRepository.GetByIdsAsync(req.ExtraServiceIds, cancellationToken);
            extraServicesPrice = extraServices.Sum(e => e.Price);
        }

        // 5. Итоговая стоимость с применением минимума
        var subtotal = areaPrice + bathroomsPrice + servicePrice + extraServicesPrice;
        var totalPrice = Math.Max(subtotal, minimumOrderAmount);

        return new CalculatePriceResponse
        {
            AreaPrice = Math.Round(areaPrice, 2),
            BathroomsPrice = Math.Round(bathroomsPrice, 2),
            ServicePrice = Math.Round(servicePrice, 2),
            ExtraServicesPrice = Math.Round(extraServicesPrice, 2),
            Subtotal = Math.Round(subtotal, 2),
            MinimumOrderAmount = minimumOrderAmount,
            TotalPrice = Math.Round(totalPrice, 2)
        };
    }
}
