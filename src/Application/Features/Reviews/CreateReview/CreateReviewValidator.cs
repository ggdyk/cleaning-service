using Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Features.Reviews.CreateReview;

public class CreateReviewValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewValidator(IStringLocalizer<ValidationMessages> L)
    {
        RuleFor(x => x.AuthorName)
            .NotEmpty().WithMessage(L["AuthorNameRequired"])
            .MaximumLength(200).WithMessage(L["AuthorNameMaxLength"]);

        RuleFor(x => x.Request.Rating)
            .InclusiveBetween(1, 5).WithMessage(L["RatingRange"]);

        RuleFor(x => x.Request.ReviewText)
            .NotEmpty().WithMessage(L["ReviewTextRequired"])
            .MaximumLength(4000).WithMessage(L["ReviewTextMaxLength"]);
    }
}
