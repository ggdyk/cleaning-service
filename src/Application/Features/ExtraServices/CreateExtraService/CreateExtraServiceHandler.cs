using Application.DTOs.ExtraServices;
using Application.Features.ExtraServices.GetExtraServices;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.ExtraServices.CreateExtraService;

public class CreateExtraServiceHandler : IRequestHandler<CreateExtraServiceCommand, ExtraServiceDto>
{
    private readonly IExtraServiceRepository _repository;

    public CreateExtraServiceHandler(IExtraServiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<ExtraServiceDto> Handle(CreateExtraServiceCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;

        var extraService = ExtraService.Create(
            name: req.Name,
            description: req.Description,
            price: req.Price,
            unit: req.Unit);

        await _repository.AddAsync(extraService, cancellationToken);

        return GetExtraServicesHandler.ToDto(extraService);
    }
}
