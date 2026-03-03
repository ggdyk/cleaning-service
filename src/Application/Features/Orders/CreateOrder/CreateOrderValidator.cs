using Application.DTOs.Orders;
using Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Features.Orders.CreateOrder;

public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator(IStringLocalizer<ValidationMessages> L)
    {
        RuleFor(x => x.CityId)
            .GreaterThan(0).WithMessage(L["CityRequired"]);

        RuleFor(x => x.TimeSlotId)
            .GreaterThan(0).WithMessage(L["TimeSlotRequired"]);

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage(L["StreetRequired"])
            .MaximumLength(255).WithMessage(L["StreetMaxLength"]);

        RuleFor(x => x.House)
            .NotEmpty().WithMessage(L["HouseRequired"])
            .MaximumLength(50).WithMessage(L["HouseMaxLength"]);

        RuleFor(x => x.Apartment)
            .MaximumLength(20).WithMessage(L["ApartmentMaxLength"])
            .When(x => x.Apartment != null);

        RuleFor(x => x.Area)
            .GreaterThan(0).WithMessage(L["AreaPositive"]);

        RuleFor(x => x.Bathrooms)
            .GreaterThanOrEqualTo(0).WithMessage(L["BathroomsNonNegative"]);

        RuleFor(x => x.Comment)
            .MaximumLength(1000).WithMessage(L["CommentMaxLength"])
            .When(x => x.Comment != null);

        RuleFor(x => x.Services)
            .NotEmpty().WithMessage(L["ServicesRequired"]);

        RuleForEach(x => x.Services).ChildRules(service =>
        {
            service.RuleFor(s => s.ServiceId)
                .GreaterThan(0).WithMessage(L["ServiceIdInvalid"]);

            service.RuleFor(s => s.Quantity)
                .GreaterThan(0).WithMessage(L["QuantityPositive"]);
        });

        RuleForEach(x => x.ExtraServices).ChildRules(extra =>
        {
            extra.RuleFor(e => e.ExtraServiceId)
                .GreaterThan(0).WithMessage(L["ExtraServiceIdInvalid"]);

            extra.RuleFor(e => e.Quantity)
                .GreaterThan(0).WithMessage(L["QuantityPositive"]);
        });
    }
}
