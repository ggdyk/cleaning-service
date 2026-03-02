using FluentValidation;

namespace Application.Features.Reviews.CreateReview;

public class CreateReviewValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewValidator()
    {
        RuleFor(x => x.AuthorName)
            .NotEmpty().WithMessage("Имя автора обязательно.")
            .MaximumLength(200).WithMessage("Имя автора не должно превышать 200 символов.");

        RuleFor(x => x.Request.Rating)
            .InclusiveBetween(1, 5).WithMessage("Оценка должна быть от 1 до 5.");

        RuleFor(x => x.Request.ReviewText)
            .NotEmpty().WithMessage("Текст отзыва обязателен.")
            .MaximumLength(4000).WithMessage("Текст отзыва не должен превышать 4000 символов.");
    }
}
