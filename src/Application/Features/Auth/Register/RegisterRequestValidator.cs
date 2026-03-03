using Application.DTOs.Auth;
using Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Features.Auth.Register;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator(IStringLocalizer<ValidationMessages> L)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(L["EmailRequired"])
            .EmailAddress().WithMessage(L["EmailInvalidFormat"])
            .MaximumLength(255).WithMessage(L["EmailMaxLength"]);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(L["PasswordRequired"])
            .MinimumLength(8).WithMessage(L["PasswordMinLength"])
            .MaximumLength(100).WithMessage(L["PasswordMaxLength"])
            .Matches(@"[A-Z]").WithMessage(L["PasswordUpperCase"])
            .Matches(@"[a-z]").WithMessage(L["PasswordLowerCase"])
            .Matches(@"[0-9]").WithMessage(L["PasswordDigit"]);

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(L["FirstNameRequired"])
            .MaximumLength(100).WithMessage(L["FirstNameMaxLength"]);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(L["LastNameRequired"])
            .MaximumLength(100).WithMessage(L["LastNameMaxLength"]);

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage(L["PhoneRequired"])
            .MinimumLength(10).WithMessage(L["PhoneMinLength"])
            .MaximumLength(20).WithMessage(L["PhoneMaxLength"]);

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage(L["CityMaxLength"])
            .When(x => !string.IsNullOrEmpty(x.City));
    }
}
