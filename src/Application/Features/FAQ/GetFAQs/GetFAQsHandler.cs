using Application.DTOs.FAQ;
using Application.Interfaces;
using MediatR;

namespace Application.Features.FAQ.GetFAQs;

public class GetFAQsHandler : IRequestHandler<GetFAQsQuery, IReadOnlyList<FaqResponse>>
{
    private readonly IFAQRepository _faqRepository;

    public GetFAQsHandler(IFAQRepository faqRepository)
    {
        _faqRepository = faqRepository;
    }

    public async Task<IReadOnlyList<FaqResponse>> Handle(
        GetFAQsQuery request,
        CancellationToken cancellationToken)
    {
        var faqs = await _faqRepository.GetActiveAsync();

        return faqs.Select(f => new FaqResponse
        {
            Id = f.Id,
            QuestionRu = f.QuestionRu,
            QuestionKk = f.QuestionKk,
            QuestionEn = f.QuestionEn,
            AnswerRu = f.AnswerRu,
            AnswerKk = f.AnswerKk,
            AnswerEn = f.AnswerEn,
            SortOrder = f.SortOrder,
            IsActive = f.IsActive,
            CreatedAt = f.CreatedAt,
            UpdatedAt = f.UpdatedAt
        }).ToList();
    }
}
