using Application.DTOs.FAQ;
using Application.Interfaces;
using MediatR;
using FaqEntity = Domain.Entities.FAQ;

namespace Application.Features.FAQ.Admin.CreateFAQ;

public class CreateFAQHandler : IRequestHandler<CreateFAQCommand, FaqResponse>
{
    private readonly IFAQRepository _faqRepository;

    public CreateFAQHandler(IFAQRepository faqRepository)
    {
        _faqRepository = faqRepository;
    }

    public async Task<FaqResponse> Handle(
        CreateFAQCommand command,
        CancellationToken cancellationToken)
    {
        var req = command.Request;

        var faq = FaqEntity.Create(
            questionRu: req.QuestionRu,
            questionKk: req.QuestionKk,
            questionEn: req.QuestionEn,
            answerRu: req.AnswerRu,
            answerKk: req.AnswerKk,
            answerEn: req.AnswerEn,
            sortOrder: req.SortOrder);

        await _faqRepository.AddAsync(faq);

        return new FaqResponse
        {
            Id = faq.Id,
            QuestionRu = faq.QuestionRu,
            QuestionKk = faq.QuestionKk,
            QuestionEn = faq.QuestionEn,
            AnswerRu = faq.AnswerRu,
            AnswerKk = faq.AnswerKk,
            AnswerEn = faq.AnswerEn,
            SortOrder = faq.SortOrder,
            IsActive = faq.IsActive,
            CreatedAt = faq.CreatedAt,
            UpdatedAt = faq.UpdatedAt
        };
    }
}
