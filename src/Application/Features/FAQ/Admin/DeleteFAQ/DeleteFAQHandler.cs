using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.FAQ.Admin.DeleteFAQ;

public class DeleteFAQHandler : IRequestHandler<DeleteFAQCommand>
{
    private readonly IFAQRepository _faqRepository;

    public DeleteFAQHandler(IFAQRepository faqRepository)
    {
        _faqRepository = faqRepository;
    }

    public async Task Handle(DeleteFAQCommand command, CancellationToken cancellationToken)
    {
        var faq = await _faqRepository.GetByIdAsync(command.Id)
            ?? throw new NotFoundException("FAQ", command.Id);

        await _faqRepository.DeleteAsync(faq);
    }
}
