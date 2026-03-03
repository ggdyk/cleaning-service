using Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Features.ExtraServices.UpdateExtraService;

public class UpdateExtraServiceValidator : AbstractValidator<UpdateExtraServiceCommand>
{
    public UpdateExtraServiceValidator(IStringLocalizer<ValidationMessages> L)
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage(L["TitleRequired"])
            .MaximumLength(255);

        RuleFor(x => x.Request.Description)
            .NotEmpty().WithMessage(L["DescriptionRequired"])
            .MaximumLength(1000);

        RuleFor(x => x.Request.Price)
            .GreaterThanOrEqualTo(0).WithMessage(L["PriceNonNegative"]);

        RuleFor(x => x.Request.Unit)
            .NotEmpty().WithMessage(L["UnitRequired"])
            .MaximumLength(50);
    }
}
