using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.ExtraServices.DeleteExtraService;

public class DeleteExtraServiceHandler : IRequestHandler<DeleteExtraServiceCommand>
{
    private readonly IExtraServiceRepository _repository;

    public DeleteExtraServiceHandler(IExtraServiceRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteExtraServiceCommand command, CancellationToken cancellationToken)
    {
        var extraService = await _repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException("ExtraService", command.Id);

        await _repository.DeleteAsync(extraService, cancellationToken);
    }
}
