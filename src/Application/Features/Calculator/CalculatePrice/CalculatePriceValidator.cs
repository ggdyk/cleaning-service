using FluentValidation;

namespace Application.Features.Calculator.CalculatePrice;

public class CalculatePriceValidator : AbstractValidator<CalculatePriceQuery>
{
    public CalculatePriceValidator()
    {
        RuleFor(x => x.Request.CityId)
            .GreaterThan(0).WithMessage("CityId должен быть положительным числом.");

        RuleFor(x => x.Request.Area)
            .GreaterThan(0).WithMessage("Площадь должна быть больше нуля.")
            .LessThanOrEqualTo(10_000).WithMessage("Площадь не может превышать 10 000 кв.м.");

        RuleFor(x => x.Request.Bathrooms)
            .GreaterThanOrEqualTo(0).WithMessage("Количество санузлов не может быть отрицательным.")
            .LessThanOrEqualTo(20).WithMessage("Количество санузлов не может превышать 20.");

        RuleFor(x => x.Request.ServiceId)
            .GreaterThan(0).When(x => x.Request.ServiceId.HasValue)
            .WithMessage("ServiceId должен быть положительным числом.");

        RuleFor(x => x.Request.ExtraServiceIds)
            .Must(ids => ids.All(id => id > 0))
            .WithMessage("Все идентификаторы дополнительных услуг должны быть положительными.")
            .When(x => x.Request.ExtraServiceIds.Count > 0);
    }
}
