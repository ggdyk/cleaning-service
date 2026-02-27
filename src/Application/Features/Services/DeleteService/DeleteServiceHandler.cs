using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Services.DeleteService;

public class DeleteServiceHandler : IRequestHandler<DeleteServiceCommand>
{
    private readonly IServiceRepository _serviceRepository;

    public DeleteServiceHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.Id, cancellationToken);

        if (service is null)
            throw new NotFoundException("Услуга", request.Id);

        await _serviceRepository.DeleteAsync(service, cancellationToken);
    }
}
