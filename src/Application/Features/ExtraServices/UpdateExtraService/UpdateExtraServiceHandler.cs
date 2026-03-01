using Application.DTOs.ExtraServices;
using Application.Features.ExtraServices.GetExtraServices;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.ExtraServices.UpdateExtraService;

public class UpdateExtraServiceHandler : IRequestHandler<UpdateExtraServiceCommand, ExtraServiceDto>
{
    private readonly IExtraServiceRepository _repository;

    public UpdateExtraServiceHandler(IExtraServiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<ExtraServiceDto> Handle(UpdateExtraServiceCommand command, CancellationToken cancellationToken)
    {
        var extraService = await _repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException("ExtraService", command.Id);

        var req = command.Request;
        extraService.Update(
            name: req.Name,
            description: req.Description,
            price: req.Price,
            unit: req.Unit,
            isActive: req.IsActive);

        await _repository.UpdateAsync(extraService, cancellationToken);

        return GetExtraServicesHandler.ToDto(extraService);
    }
}
