using Application.DTOs.FAQ;
using Application.Interfaces;
using MediatR;

namespace Application.Features.FAQ.Admin.GetAllFAQs;

public class GetAllFAQsHandler : IRequestHandler<GetAllFAQsQuery, IReadOnlyList<FaqResponse>>
{
    private readonly IFAQRepository _faqRepository;

    public GetAllFAQsHandler(IFAQRepository faqRepository)
    {
        _faqRepository = faqRepository;
    }

    public async Task<IReadOnlyList<FaqResponse>> Handle(
        GetAllFAQsQuery request,
        CancellationToken cancellationToken)
    {
        var faqs = await _faqRepository.GetAllAsync();

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
