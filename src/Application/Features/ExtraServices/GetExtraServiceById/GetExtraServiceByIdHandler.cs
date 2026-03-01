using Application.DTOs.ExtraServices;
using Application.Features.ExtraServices.GetExtraServices;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.ExtraServices.GetExtraServiceById;

public class GetExtraServiceByIdHandler : IRequestHandler<GetExtraServiceByIdQuery, ExtraServiceDto>
{
    private readonly IExtraServiceRepository _repository;

    public GetExtraServiceByIdHandler(IExtraServiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<ExtraServiceDto> Handle(GetExtraServiceByIdQuery query, CancellationToken cancellationToken)
    {
        var extraService = await _repository.GetByIdAsync(query.Id, cancellationToken)
            ?? throw new NotFoundException("ExtraService", query.Id);

        return GetExtraServicesHandler.ToDto(extraService);
    }
}
