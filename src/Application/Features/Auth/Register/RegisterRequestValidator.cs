using Application.DTOs.Auth;
using Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Features.Auth.Register;

public class RegisterRequestValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterRequestValidator(IStringLocalizer<ValidationMessages> L)
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage(L["EmailRequired"])
            .EmailAddress().WithMessage(L["EmailInvalidFormat"])
            .MaximumLength(255).WithMessage(L["EmailMaxLength"]);

        RuleFor(x => x.Request.Password)
            .NotEmpty().WithMessage(L["PasswordRequired"])
            .MinimumLength(8).WithMessage(L["PasswordMinLength"])
            .MaximumLength(100).WithMessage(L["PasswordMaxLength"])
            .Matches(@"[A-Z]").WithMessage(L["PasswordUpperCase"])
            .Matches(@"[a-z]").WithMessage(L["PasswordLowerCase"])
            .Matches(@"[0-9]").WithMessage(L["PasswordDigit"]);

        RuleFor(x => x.Request.FirstName)
            .NotEmpty().WithMessage(L["FirstNameRequired"])
            .MaximumLength(100).WithMessage(L["FirstNameMaxLength"]);

        RuleFor(x => x.Request.LastName)
            .NotEmpty().WithMessage(L["LastNameRequired"])
            .MaximumLength(100).WithMessage(L["LastNameMaxLength"]);

        RuleFor(x => x.Request.Phone)
            .NotEmpty().WithMessage(L["PhoneRequired"])
            .MinimumLength(10).WithMessage(L["PhoneMinLength"])
            .MaximumLength(20).WithMessage(L["PhoneMaxLength"]);

        RuleFor(x => x.Request.City)
            .MaximumLength(100).WithMessage(L["CityMaxLength"])
            .When(x => !string.IsNullOrEmpty(x.Request.City));
    }
}
