using FluentValidation;

namespace Application.Features.FAQ.Admin.CreateFAQ;

public class CreateFAQValidator : AbstractValidator<CreateFAQCommand>
{
    public CreateFAQValidator()
    {
        RuleFor(x => x.Request.QuestionRu)
            .NotEmpty().WithMessage("Вопрос на русском обязателен.")
            .MaximumLength(1000);

        RuleFor(x => x.Request.QuestionKk)
            .NotEmpty().WithMessage("Вопрос на казахском обязателен.")
            .MaximumLength(1000);

        RuleFor(x => x.Request.QuestionEn)
            .NotEmpty().WithMessage("Вопрос на английском обязателен.")
            .MaximumLength(1000);

        RuleFor(x => x.Request.AnswerRu)
            .NotEmpty().WithMessage("Ответ на русском обязателен.")
            .MaximumLength(5000);

        RuleFor(x => x.Request.AnswerKk)
            .NotEmpty().WithMessage("Ответ на казахском обязателен.")
            .MaximumLength(5000);

        RuleFor(x => x.Request.AnswerEn)
            .NotEmpty().WithMessage("Ответ на английском обязателен.")
            .MaximumLength(5000);

        RuleFor(x => x.Request.SortOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Порядок сортировки не может быть отрицательным.");
    }
}
