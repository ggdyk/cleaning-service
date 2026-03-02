using Application.Common;
using Application.DTOs.FAQ;
using Application.Interfaces;
using MediatR;

namespace Application.Features.FAQ.GetFAQs;

public class GetFAQsHandler : IRequestHandler<GetFAQsQuery, IReadOnlyList<FaqLocalizedResponse>>
{
    private readonly IFAQRepository _faqRepository;
    private readonly ILanguageContext _languageContext;

    public GetFAQsHandler(IFAQRepository faqRepository, ILanguageContext languageContext)
    {
        _faqRepository = faqRepository;
        _languageContext = languageContext;
    }

    public async Task<IReadOnlyList<FaqLocalizedResponse>> Handle(
        GetFAQsQuery request,
        CancellationToken cancellationToken)
    {
        var faqs = await _faqRepository.GetActiveAsync();
        var lang = _languageContext.Language;

        return faqs.Select(f => new FaqLocalizedResponse
        {
            Id = f.Id,
            Question = LocalizationHelper.Pick(f.QuestionRu, f.QuestionKk, f.QuestionEn, lang),
            Answer = LocalizationHelper.Pick(f.AnswerRu, f.AnswerKk, f.AnswerEn, lang),
            SortOrder = f.SortOrder
        }).ToList();
    }
}
