using FluentValidation;

namespace Application.Features.ExtraServices.UpdateExtraService;

public class UpdateExtraServiceValidator : AbstractValidator<UpdateExtraServiceCommand>
{
    public UpdateExtraServiceValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage("Название обязательно.")
            .MaximumLength(255);

        RuleFor(x => x.Request.Description)
            .NotEmpty().WithMessage("Описание обязательно.")
            .MaximumLength(1000);

        RuleFor(x => x.Request.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Цена не может быть отрицательной.");

        RuleFor(x => x.Request.Unit)
            .NotEmpty().WithMessage("Единица измерения обязательна.")
            .MaximumLength(50);
    }
}
