using Application.DTOs.FAQ;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.FAQ.Admin.UpdateFAQ;

public class UpdateFAQHandler : IRequestHandler<UpdateFAQCommand, FaqResponse>
{
    private readonly IFAQRepository _faqRepository;

    public UpdateFAQHandler(IFAQRepository faqRepository)
    {
        _faqRepository = faqRepository;
    }

    public async Task<FaqResponse> Handle(
        UpdateFAQCommand command,
        CancellationToken cancellationToken)
    {
        var faq = await _faqRepository.GetByIdAsync(command.Id)
            ?? throw new NotFoundException("FAQ", command.Id);

        var req = command.Request;

        faq.Update(
            questionRu: req.QuestionRu,
            questionKk: req.QuestionKk,
            questionEn: req.QuestionEn,
            answerRu: req.AnswerRu,
            answerKk: req.AnswerKk,
            answerEn: req.AnswerEn,
            sortOrder: req.SortOrder);

        await _faqRepository.UpdateAsync(faq);

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
