using FluentValidation;

namespace Application.Features.Callbacks.SubmitCallbackRequest;

public class SubmitCallbackRequestValidator : AbstractValidator<SubmitCallbackRequestCommand>
{
    public SubmitCallbackRequestValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage("Имя обязательно.")
            .MaximumLength(200).WithMessage("Имя не должно превышать 200 символов.");

        RuleFor(x => x.Request.Phone)
            .NotEmpty().WithMessage("Номер телефона обязателен.")
            .MaximumLength(30).WithMessage("Номер телефона не должен превышать 30 символов.")
            .Matches(@"^[\d\+\-\(\)\s]+$").WithMessage("Номер телефона содержит недопустимые символы.");

        RuleFor(x => x.Request.PreferredTime)
            .MaximumLength(200).WithMessage("Удобное время не должно превышать 200 символов.")
            .When(x => x.Request.PreferredTime != null);
    }
}
