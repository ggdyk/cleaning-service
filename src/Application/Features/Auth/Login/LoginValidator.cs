using FluentValidation;

namespace Application.Features.Auth.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Email обязателен.")
            .EmailAddress().WithMessage("Неправильный формат email.");

        RuleFor(x => x.Request.Password)
            .NotEmpty().WithMessage("Пароль обязателен.");
    }
}
