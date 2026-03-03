using Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Features.FAQ.Admin.UpdateFAQ;

public class UpdateFAQValidator : AbstractValidator<UpdateFAQCommand>
{
    public UpdateFAQValidator(IStringLocalizer<ValidationMessages> L)
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage(L["InvalidFaqId"]);

        RuleFor(x => x.Request.QuestionRu)
            .NotEmpty().WithMessage(L["FaqQuestionRuRequired"])
            .MaximumLength(1000);

        RuleFor(x => x.Request.QuestionKk)
            .NotEmpty().WithMessage(L["FaqQuestionKkRequired"])
            .MaximumLength(1000);

        RuleFor(x => x.Request.QuestionEn)
            .NotEmpty().WithMessage(L["FaqQuestionEnRequired"])
            .MaximumLength(1000);

        RuleFor(x => x.Request.AnswerRu)
            .NotEmpty().WithMessage(L["FaqAnswerRuRequired"])
            .MaximumLength(5000);

        RuleFor(x => x.Request.AnswerKk)
            .NotEmpty().WithMessage(L["FaqAnswerKkRequired"])
            .MaximumLength(5000);

        RuleFor(x => x.Request.AnswerEn)
            .NotEmpty().WithMessage(L["FaqAnswerEnRequired"])
            .MaximumLength(5000);

        RuleFor(x => x.Request.SortOrder)
            .GreaterThanOrEqualTo(0).WithMessage(L["SortOrderNonNegative"]);
    }
}
