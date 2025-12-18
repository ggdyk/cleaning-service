using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Features.Auth.Register;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Неправильный формат email")
            .MaximumLength(255).WithMessage("Email не может превышать 255 символов");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(8).WithMessage("Пароль должен быть минимум 8 символов")
            .MaximumLength(100).WithMessage("Пароль не может превышать 100 символов")
            .Matches(@"[A-Z]").WithMessage("Пароль должен содержать минимум 1 заглавную букву")
            .Matches(@"[a-z]").WithMessage("Пароль должен содержать минимум 1 строчную букву")
            .Matches(@"[0-9]").WithMessage("Пароль должен содержать минимум 1 цифру");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Имя обязательно")
            .MaximumLength(100).WithMessage("Имя не может превышать 100 символов");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Фамилия обязательна")
            .MaximumLength(100).WithMessage("Фамилия не может превышать 100 символов");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Телефон обязателен")
            .MinimumLength(10).WithMessage("Телефон должен быть минимум 10 символов")
            .MaximumLength(20).WithMessage("Телефон не может превышать 20 символов");

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("Город не может превышать 100 символов")
            .When(x => !string.IsNullOrEmpty(x.City));
    }
}