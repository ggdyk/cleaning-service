using Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Features.Callbacks.SubmitCallbackRequest;

public class SubmitCallbackRequestValidator : AbstractValidator<SubmitCallbackRequestCommand>
{
    public SubmitCallbackRequestValidator(IStringLocalizer<ValidationMessages> L)
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage(L["NameRequired"])
            .MaximumLength(200).WithMessage(L["NameMaxLength"]);

        RuleFor(x => x.Request.Phone)
            .NotEmpty().WithMessage(L["PhoneCallbackRequired"])
            .MaximumLength(30).WithMessage(L["PhoneCallbackMaxLength"])
            .Matches(@"^[\d\+\-\(\)\s]+$").WithMessage(L["PhoneInvalidChars"]);

        RuleFor(x => x.Request.PreferredTime)
            .MaximumLength(200).WithMessage(L["PreferredTimeMaxLength"])
            .When(x => x.Request.PreferredTime != null);
    }
}
