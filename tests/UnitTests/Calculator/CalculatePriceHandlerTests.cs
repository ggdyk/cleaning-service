using Application.DTOs.Calculator;
using Application.Features.Calculator.CalculatePrice;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Options;
using Moq;

namespace UnitTests.Calculator;

/// <summary>
/// Тесты Application-логики CalculatePriceHandler.
/// Репозитории мокируем — тесты изолированы от БД.
/// </summary>
public class CalculatePriceHandlerTests
{
    // Дефолтные значения из конфига (fallback когда нет записей в БД)
    private const decimal DefaultPricePerSqm    = 50m;
    private const decimal DefaultPricePerBathroom = 1000m;
    private const decimal DefaultMinimum         = 3000m;

    private readonly Mock<ICalculatorSettingsRepository> _settingsRepo;
    private readonly Mock<IServiceRepository>            _serviceRepo;
    private readonly Mock<IExtraServiceRepository>       _extraServiceRepo;
    private readonly CalculatorDefaultSettings           _configDefaults;

    public CalculatePriceHandlerTests()
    {
        _settingsRepo     = new Mock<ICalculatorSettingsRepository>();
        _serviceRepo      = new Mock<IServiceRepository>();
        _extraServiceRepo = new Mock<IExtraServiceRepository>();

        _configDefaults = new CalculatorDefaultSettings
        {
            PricePerSquareMeter = DefaultPricePerSqm,
            PricePerBathroom    = DefaultPricePerBathroom,
            MinimumOrderAmount  = DefaultMinimum
        };

        // По умолчанию: нет настроек в БД ни для города, ни глобальных
        _settingsRepo
            .Setup(r => r.GetByCityIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CalculatorSettings?)null);
        _settingsRepo
            .Setup(r => r.GetDefaultAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CalculatorSettings?)null);

        // По умолчанию: дополнительных услуг нет
        _extraServiceRepo
            .Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
    }

    private CalculatePriceHandler CreateHandler() =>
        new(_settingsRepo.Object,
            _serviceRepo.Object,
            _extraServiceRepo.Object,
            Options.Create(_configDefaults));

    // =========================================================================
    // Fallback / настройки
    // =========================================================================

    [Fact]
    public async Task Handle_NoSettingsInDb_UsesConfigDefaults()
    {
        // 60 кв.м × 50 + 1 санузел × 1000 = 3000 + 1000 = 4000 > minimum 3000
        var result = await CreateHandler().Handle(
            Query(area: 60, bathrooms: 1), CancellationToken.None);

        Assert.Equal(3000m, result.AreaPrice);       // 60 × 50
        Assert.Equal(1000m, result.BathroomsPrice);  // 1 × 1000
        Assert.Equal(4000m, result.Subtotal);
        Assert.Equal(DefaultMinimum, result.MinimumOrderAmount);
        Assert.Equal(4000m, result.TotalPrice);
    }

    [Fact]
    public async Task Handle_CitySettingsFound_UsesCityRates()
    {
        // Для города настроены свои коэффициенты
        var citySettings = CalculatorSettings.Create(
            cityId: 5, pricePerSquareMeter: 100m, pricePerBathroom: 2000m, minimumOrderAmount: 8000m);

        _settingsRepo
            .Setup(r => r.GetByCityIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(citySettings);

        var result = await CreateHandler().Handle(
            Query(cityId: 5, area: 50, bathrooms: 1), CancellationToken.None);

        // 50 × 100 + 1 × 2000 = 7000 < minimum 8000 → TotalPrice = 8000
        Assert.Equal(5000m, result.AreaPrice);
        Assert.Equal(2000m, result.BathroomsPrice);
        Assert.Equal(7000m, result.Subtotal);
        Assert.Equal(8000m, result.TotalPrice);
    }

    [Fact]
    public async Task Handle_DbDefaultFound_UsesDbDefault_WhenNoCitySettings()
    {
        // Нет настроек для города, но есть глобальный дефолт в БД
        var dbDefault = CalculatorSettings.Create(
            cityId: 999, pricePerSquareMeter: 80m, pricePerBathroom: 1200m, minimumOrderAmount: 4000m);

        _settingsRepo
            .Setup(r => r.GetDefaultAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbDefault);

        var result = await CreateHandler().Handle(
            Query(area: 40, bathrooms: 0), CancellationToken.None);

        // 40 × 80 = 3200, bathrooms = 0, subtotal = 3200 < minimum 4000
        Assert.Equal(3200m, result.AreaPrice);
        Assert.Equal(0m, result.BathroomsPrice);
        Assert.Equal(4000m, result.TotalPrice);
    }

    // =========================================================================
    // Минимальная сумма заказа
    // =========================================================================

    [Fact]
    public async Task Handle_SubtotalBelowMinimum_TotalEqualsMinimum()
    {
        // 10 кв.м × 50 = 500, bathrooms = 0 → subtotal = 500 < minimum 3000
        var result = await CreateHandler().Handle(
            Query(area: 10, bathrooms: 0), CancellationToken.None);

        Assert.Equal(500m,   result.Subtotal);
        Assert.Equal(3000m,  result.TotalPrice);  // применён минимум
    }

    [Fact]
    public async Task Handle_SubtotalAboveMinimum_TotalEqualsSubtotal()
    {
        // 100 кв.м × 50 + 2 × 1000 = 5000 + 2000 = 7000 > minimum 3000
        var result = await CreateHandler().Handle(
            Query(area: 100, bathrooms: 2), CancellationToken.None);

        Assert.Equal(7000m, result.Subtotal);
        Assert.Equal(7000m, result.TotalPrice);
    }

    // =========================================================================
    // Основная услуга
    // =========================================================================

    [Fact]
    public async Task Handle_WithServiceId_AddsServiceBasePrice()
    {
        var service = BuildService(basePrice: 800m);
        _serviceRepo
            .Setup(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(service);

        // area = 40 × 50 = 2000, bathrooms = 0, service = 800 → subtotal = 2800 < 3000 → total = 3000
        var result = await CreateHandler().Handle(
            Query(area: 40, bathrooms: 0, serviceId: 42), CancellationToken.None);

        Assert.Equal(800m,  result.ServicePrice);
        Assert.Equal(2800m, result.Subtotal);
        Assert.Equal(3000m, result.TotalPrice);   // minimum kicks in
    }

    [Fact]
    public async Task Handle_NoServiceId_ServicePriceIsZero()
    {
        var result = await CreateHandler().Handle(
            Query(area: 60, bathrooms: 0, serviceId: null), CancellationToken.None);

        Assert.Equal(0m, result.ServicePrice);
    }

    // =========================================================================
    // Дополнительные услуги
    // =========================================================================

    [Fact]
    public async Task Handle_WithExtraServices_AddsTheirSum()
    {
        var extras = new List<ExtraService>
        {
            BuildExtraService(price: 300m),
            BuildExtraService(price: 700m)
        };
        _extraServiceRepo
            .Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(extras);

        // area = 60 × 50 = 3000, bathrooms = 0, extras = 1000 → subtotal = 4000
        var result = await CreateHandler().Handle(
            Query(area: 60, bathrooms: 0, extraServiceIds: [1, 2]), CancellationToken.None);

        Assert.Equal(1000m, result.ExtraServicesPrice);
        Assert.Equal(4000m, result.Subtotal);
        Assert.Equal(4000m, result.TotalPrice);
    }

    [Fact]
    public async Task Handle_NoExtraServices_ExtraServicesPriceIsZero()
    {
        var result = await CreateHandler().Handle(
            Query(area: 60, bathrooms: 0), CancellationToken.None);

        Assert.Equal(0m, result.ExtraServicesPrice);
    }

    // =========================================================================
    // Все компоненты вместе
    // =========================================================================

    [Fact]
    public async Task Handle_AllComponents_CalculatesTotalCorrectly()
    {
        // area = 80 × 50 = 4000
        // bathrooms = 2 × 1000 = 2000
        // service = 500
        // extras = 300 + 200 = 500
        // subtotal = 7000 > minimum 3000
        var service = BuildService(basePrice: 500m);
        _serviceRepo
            .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(service);

        _extraServiceRepo
            .Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([BuildExtraService(300m), BuildExtraService(200m)]);

        var result = await CreateHandler().Handle(
            Query(area: 80, bathrooms: 2, serviceId: 10, extraServiceIds: [20, 21]),
            CancellationToken.None);

        Assert.Equal(4000m, result.AreaPrice);
        Assert.Equal(2000m, result.BathroomsPrice);
        Assert.Equal(500m,  result.ServicePrice);
        Assert.Equal(500m,  result.ExtraServicesPrice);
        Assert.Equal(7000m, result.Subtotal);
        Assert.Equal(7000m, result.TotalPrice);
    }

    // =========================================================================
    // Исключения
    // =========================================================================

    [Fact]
    public async Task Handle_ServiceNotFound_ThrowsNotFoundException()
    {
        _serviceRepo
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Service?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateHandler().Handle(Query(serviceId: 99), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ServiceIsInactive_ThrowsBusinessRuleException()
    {
        var service = BuildService(basePrice: 500m);
        service.Deactivate();  // IsActive → false

        _serviceRepo
            .Setup(r => r.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(service);

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateHandler().Handle(Query(serviceId: 7), CancellationToken.None));
    }

    // =========================================================================
    // Вспомогательные методы
    // =========================================================================

    private static CalculatePriceQuery Query(
        int cityId = 1,
        double area = 60,
        int bathrooms = 0,
        int? serviceId = null,
        List<int>? extraServiceIds = null) =>
        new(new CalculatePriceRequest
        {
            CityId          = cityId,
            Area            = area,
            Bathrooms       = bathrooms,
            ServiceId       = serviceId,
            ExtraServiceIds = extraServiceIds ?? []
        });

    private static Service BuildService(decimal basePrice) =>
        Service.Create(
            categoryId: 1,
            nameRu: "Тест", nameEn: "Test",
            descriptionRu: "Описание", descriptionEn: "Description",
            basePrice: basePrice,
            unit: "service",
            minArea: null,
            durationMinutes: null,
            sortOrder: 1);

    private static ExtraService BuildExtraService(decimal price) =>
        ExtraService.Create("Доп. услуга", "Описание", price, "шт");
}
