using Domain.Entities;
using Domain.Exceptions;

namespace UnitTests.Calculator;

/// <summary>
/// Тесты бизнес-правил Domain-сущности CalculatorSettings.
/// Проверяем: создание, валидацию входных данных, метод Update.
/// </summary>
public class CalculatorSettingsTests
{
    // =========================================================================
    // Create — happy path
    // =========================================================================

    [Fact]
    public void Create_ValidParameters_SetsPropertiesCorrectly()
    {
        var settings = CalculatorSettings.Create(
            cityId: 1,
            pricePerSquareMeter: 75m,
            pricePerBathroom: 1500m,
            minimumOrderAmount: 5000m);

        Assert.Equal(1, settings.CityId);
        Assert.Equal(75m, settings.PricePerSquareMeter);
        Assert.Equal(1500m, settings.PricePerBathroom);
        Assert.Equal(5000m, settings.MinimumOrderAmount);
    }

    [Fact]
    public void Create_ZeroPricePerBathroom_IsAllowed()
    {
        // Бесплатные санузлы — допустимый случай (pricePerBathroom >= 0)
        var settings = CalculatorSettings.Create(
            cityId: 1,
            pricePerSquareMeter: 50m,
            pricePerBathroom: 0m,
            minimumOrderAmount: 0m);

        Assert.Equal(0m, settings.PricePerBathroom);
        Assert.Equal(0m, settings.MinimumOrderAmount);
    }

    // =========================================================================
    // Create — граничные случаи / ошибки
    // =========================================================================

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_InvalidCityId_ThrowsBusinessRuleException(int cityId)
    {
        Assert.Throws<BusinessRuleException>(() =>
            CalculatorSettings.Create(cityId, 50m, 1000m, 3000m));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void Create_PricePerSquareMeterZeroOrNegative_ThrowsBusinessRuleException(decimal price)
    {
        Assert.Throws<BusinessRuleException>(() =>
            CalculatorSettings.Create(1, price, 1000m, 3000m));
    }

    [Fact]
    public void Create_PricePerBathroomNegative_ThrowsBusinessRuleException()
    {
        Assert.Throws<BusinessRuleException>(() =>
            CalculatorSettings.Create(1, 50m, -1m, 3000m));
    }

    [Fact]
    public void Create_MinimumOrderAmountNegative_ThrowsBusinessRuleException()
    {
        Assert.Throws<BusinessRuleException>(() =>
            CalculatorSettings.Create(1, 50m, 1000m, -1m));
    }

    // =========================================================================
    // Update
    // =========================================================================

    [Fact]
    public void Update_ValidParameters_ChangesProperties()
    {
        var settings = CalculatorSettings.Create(1, 50m, 1000m, 3000m);

        settings.Update(100m, 2000m, 6000m);

        Assert.Equal(100m, settings.PricePerSquareMeter);
        Assert.Equal(2000m, settings.PricePerBathroom);
        Assert.Equal(6000m, settings.MinimumOrderAmount);
    }

    [Fact]
    public void Update_PricePerSquareMeterZero_ThrowsBusinessRuleException()
    {
        var settings = CalculatorSettings.Create(1, 50m, 1000m, 3000m);

        Assert.Throws<BusinessRuleException>(() =>
            settings.Update(0m, 1000m, 3000m));
    }

    [Fact]
    public void Update_PricePerBathroomNegative_ThrowsBusinessRuleException()
    {
        var settings = CalculatorSettings.Create(1, 50m, 1000m, 3000m);

        Assert.Throws<BusinessRuleException>(() =>
            settings.Update(50m, -1m, 3000m));
    }

    [Fact]
    public void Update_MinimumOrderAmountNegative_ThrowsBusinessRuleException()
    {
        var settings = CalculatorSettings.Create(1, 50m, 1000m, 3000m);

        Assert.Throws<BusinessRuleException>(() =>
            settings.Update(50m, 1000m, -100m));
    }
}
