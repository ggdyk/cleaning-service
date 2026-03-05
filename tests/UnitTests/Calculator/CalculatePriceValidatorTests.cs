using Application.DTOs.Calculator;
using Application.Features.Calculator.CalculatePrice;
using Application.Resources;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Localization;
using Moq;

namespace UnitTests.Calculator;

/// <summary>
/// Тесты FluentValidation-правил CalculatePriceValidator.
/// Проверяем: допустимые значения пропускаются, недопустимые — отклоняются.
/// </summary>
public class CalculatePriceValidatorTests
{
    private readonly CalculatePriceValidator _validator;

    public CalculatePriceValidatorTests()
    {
        // Мокируем локализатор — возвращает ключ как сообщение
        var localizer = new Mock<IStringLocalizer<ValidationMessages>>();
        localizer
            .Setup(l => l[It.IsAny<string>()])
            .Returns((string key) => new LocalizedString(key, key));

        _validator = new CalculatePriceValidator(localizer.Object);
    }

    // =========================================================================
    // Валидный запрос
    // =========================================================================

    [Fact]
    public void Validate_ValidRequest_NoErrors()
    {
        var result = _validator.TestValidate(ValidQuery());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ServiceIdNull_NoError()
    {
        // ServiceId — опциональное поле, null = не выбрана услуга
        var result = _validator.TestValidate(ValidQuery(serviceId: null));
        result.ShouldNotHaveValidationErrorFor(x => x.Request.ServiceId);
    }

    [Fact]
    public void Validate_EmptyExtraServiceIds_NoError()
    {
        var result = _validator.TestValidate(ValidQuery(extraServiceIds: []));
        result.ShouldNotHaveValidationErrorFor(x => x.Request.ExtraServiceIds);
    }

    // =========================================================================
    // CityId
    // =========================================================================

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_CityIdNotPositive_HasError(int cityId)
    {
        var result = _validator.TestValidate(ValidQuery(cityId: cityId));
        result.ShouldHaveValidationErrorFor(x => x.Request.CityId);
    }

    // =========================================================================
    // Area
    // =========================================================================

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_AreaNotPositive_HasError(double area)
    {
        var result = _validator.TestValidate(ValidQuery(area: area));
        result.ShouldHaveValidationErrorFor(x => x.Request.Area);
    }

    [Fact]
    public void Validate_AreaExceedsMaximum_HasError()
    {
        var result = _validator.TestValidate(ValidQuery(area: 10_001));
        result.ShouldHaveValidationErrorFor(x => x.Request.Area);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10_000)]
    public void Validate_AreaOnBoundary_NoError(double area)
    {
        var result = _validator.TestValidate(ValidQuery(area: area));
        result.ShouldNotHaveValidationErrorFor(x => x.Request.Area);
    }

    // =========================================================================
    // Bathrooms
    // =========================================================================

    [Fact]
    public void Validate_BathroomsNegative_HasError()
    {
        var result = _validator.TestValidate(ValidQuery(bathrooms: -1));
        result.ShouldHaveValidationErrorFor(x => x.Request.Bathrooms);
    }

    [Fact]
    public void Validate_BathroomsExceedsMaximum_HasError()
    {
        var result = _validator.TestValidate(ValidQuery(bathrooms: 21));
        result.ShouldHaveValidationErrorFor(x => x.Request.Bathrooms);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(20)]
    public void Validate_BathroomsOnBoundary_NoError(int bathrooms)
    {
        var result = _validator.TestValidate(ValidQuery(bathrooms: bathrooms));
        result.ShouldNotHaveValidationErrorFor(x => x.Request.Bathrooms);
    }

    // =========================================================================
    // ServiceId (опциональный)
    // =========================================================================

    [Fact]
    public void Validate_ServiceIdZeroWhenProvided_HasError()
    {
        // Если serviceId передан (не null), он должен быть > 0
        var result = _validator.TestValidate(ValidQuery(serviceId: 0));
        result.ShouldHaveValidationErrorFor(x => x.Request.ServiceId);
    }

    [Fact]
    public void Validate_ServiceIdPositive_NoError()
    {
        var result = _validator.TestValidate(ValidQuery(serviceId: 5));
        result.ShouldNotHaveValidationErrorFor(x => x.Request.ServiceId);
    }

    // =========================================================================
    // ExtraServiceIds
    // =========================================================================

    [Fact]
    public void Validate_ExtraServiceIdContainsZero_HasError()
    {
        var result = _validator.TestValidate(ValidQuery(extraServiceIds: [1, 0]));
        result.ShouldHaveValidationErrorFor(x => x.Request.ExtraServiceIds);
    }

    [Fact]
    public void Validate_ExtraServiceIds_AllPositive_NoError()
    {
        var result = _validator.TestValidate(ValidQuery(extraServiceIds: [1, 2, 3]));
        result.ShouldNotHaveValidationErrorFor(x => x.Request.ExtraServiceIds);
    }

    // =========================================================================
    // Вспомогательный метод
    // =========================================================================

    private static CalculatePriceQuery ValidQuery(
        int cityId = 1,
        double area = 60,
        int bathrooms = 1,
        int? serviceId = 1,
        List<int>? extraServiceIds = null) =>
        new(new CalculatePriceRequest
        {
            CityId          = cityId,
            Area            = area,
            Bathrooms       = bathrooms,
            ServiceId       = serviceId,
            ExtraServiceIds = extraServiceIds ?? [1]
        });
}
