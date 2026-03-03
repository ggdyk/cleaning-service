using Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Features.Calculator.CalculatePrice;

public class CalculatePriceValidator : AbstractValidator<CalculatePriceQuery>
{
    public CalculatePriceValidator(IStringLocalizer<ValidationMessages> L)
    {
        RuleFor(x => x.Request.CityId)
            .GreaterThan(0).WithMessage(L["CityIdPositive"]);

        RuleFor(x => x.Request.Area)
            .GreaterThan(0).WithMessage(L["AreaPositive"])
            .LessThanOrEqualTo(10_000).WithMessage(L["AreaMaxValue"]);

        RuleFor(x => x.Request.Bathrooms)
            .GreaterThanOrEqualTo(0).WithMessage(L["BathroomsNonNegative"])
            .LessThanOrEqualTo(20).WithMessage(L["BathroomsMaxValue"]);

        RuleFor(x => x.Request.ServiceId)
            .GreaterThan(0).When(x => x.Request.ServiceId.HasValue)
            .WithMessage(L["ServiceIdPositive"]);

        RuleFor(x => x.Request.ExtraServiceIds)
            .Must(ids => ids.All(id => id > 0))
            .WithMessage(L["ExtraServiceIdsPositive"])
            .When(x => x.Request.ExtraServiceIds.Count > 0);
    }
}
