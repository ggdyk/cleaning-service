using Application.DTOs.Orders;
using FluentValidation;

namespace Application.Features.Orders.CreateOrder;

public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CityId)
            .GreaterThan(0).WithMessage("Необходимо выбрать город");

        RuleFor(x => x.TimeSlotId)
            .GreaterThan(0).WithMessage("Необходимо выбрать временной слот");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Улица обязательна")
            .MaximumLength(255).WithMessage("Улица не может превышать 255 символов");

        RuleFor(x => x.House)
            .NotEmpty().WithMessage("Номер дома обязателен")
            .MaximumLength(50).WithMessage("Номер дома не может превышать 50 символов");

        RuleFor(x => x.Apartment)
            .MaximumLength(20).WithMessage("Квартира не может превышать 20 символов")
            .When(x => x.Apartment != null);

        RuleFor(x => x.Area)
            .GreaterThan(0).WithMessage("Площадь должна быть больше нуля");

        RuleFor(x => x.Bathrooms)
            .GreaterThanOrEqualTo(0).WithMessage("Количество санузлов не может быть отрицательным");

        RuleFor(x => x.Comment)
            .MaximumLength(1000).WithMessage("Комментарий не может превышать 1000 символов")
            .When(x => x.Comment != null);

        RuleFor(x => x.Services)
            .NotEmpty().WithMessage("Необходимо выбрать хотя бы одну услугу");

        RuleForEach(x => x.Services).ChildRules(service =>
        {
            service.RuleFor(s => s.ServiceId)
                .GreaterThan(0).WithMessage("Некорректный ID услуги");

            service.RuleFor(s => s.ServiceName)
                .NotEmpty().WithMessage("Название услуги обязательно")
                .MaximumLength(255).WithMessage("Название услуги не может превышать 255 символов");

            service.RuleFor(s => s.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Цена не может быть отрицательной");

            service.RuleFor(s => s.Quantity)
                .GreaterThan(0).WithMessage("Количество должно быть больше нуля");
        });

        RuleForEach(x => x.ExtraServices).ChildRules(extra =>
        {
            extra.RuleFor(e => e.ExtraServiceId)
                .GreaterThan(0).WithMessage("Некорректный ID доп. услуги");

            extra.RuleFor(e => e.Name)
                .NotEmpty().WithMessage("Название доп. услуги обязательно")
                .MaximumLength(255).WithMessage("Название доп. услуги не может превышать 255 символов");

            extra.RuleFor(e => e.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Цена не может быть отрицательной");

            extra.RuleFor(e => e.Quantity)
                .GreaterThan(0).WithMessage("Количество должно быть больше нуля");
        });
    }
}
