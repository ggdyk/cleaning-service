using Application.DTOs.Orders;
using Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Features.Orders.CreateOrder;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator(IStringLocalizer<ValidationMessages> L)
    {
        RuleFor(x => x.Request.CityId)
            .GreaterThan(0).WithMessage(L["CityRequired"]);

        RuleFor(x => x.Request.TimeSlotId)
            .GreaterThan(0).WithMessage(L["TimeSlotRequired"]);

        RuleFor(x => x.Request.Street)
            .NotEmpty().WithMessage(L["StreetRequired"])
            .MaximumLength(255).WithMessage(L["StreetMaxLength"]);

        RuleFor(x => x.Request.House)
            .NotEmpty().WithMessage(L["HouseRequired"])
            .MaximumLength(50).WithMessage(L["HouseMaxLength"]);

        RuleFor(x => x.Request.Apartment)
            .MaximumLength(20).WithMessage(L["ApartmentMaxLength"])
            .When(x => x.Request.Apartment != null);

        RuleFor(x => x.Request.Area)
            .GreaterThan(0).WithMessage(L["AreaPositive"]);

        RuleFor(x => x.Request.Bathrooms)
            .GreaterThanOrEqualTo(0).WithMessage(L["BathroomsNonNegative"]);

        RuleFor(x => x.Request.Comment)
            .MaximumLength(1000).WithMessage(L["CommentMaxLength"])
            .When(x => x.Request.Comment != null);

        RuleFor(x => x.Request.Services)
            .NotEmpty().WithMessage(L["ServicesRequired"]);

        RuleForEach(x => x.Request.Services).ChildRules(service =>
        {
            service.RuleFor(s => s.ServiceId)
                .GreaterThan(0).WithMessage(L["ServiceIdInvalid"]);

            service.RuleFor(s => s.Quantity)
                .GreaterThan(0).WithMessage(L["QuantityPositive"]);
        });

        RuleForEach(x => x.Request.ExtraServices).ChildRules(extra =>
        {
            extra.RuleFor(e => e.ExtraServiceId)
                .GreaterThan(0).WithMessage(L["ExtraServiceIdInvalid"]);

            extra.RuleFor(e => e.Quantity)
                .GreaterThan(0).WithMessage(L["QuantityPositive"]);
        });
    }
}
