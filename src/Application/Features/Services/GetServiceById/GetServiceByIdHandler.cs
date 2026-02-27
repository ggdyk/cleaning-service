using Application.DTOs.Services;
using Application.Features.Services.GetServicesById;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Services.GetServiceById;

public class GetServiceByIdHandler : IRequestHandler<GetServiceByIdQuery, ServiceDto>
{
    private readonly IServiceRepository _serviceRepository;

    public GetServiceByIdHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<ServiceDto> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.Id, cancellationToken);

        if (service is null)
            throw new NotFoundException("Услуга", request.Id);

        return new ServiceDto
        {
            Id = service.Id,
            CategoryId = service.CategoryId,
            NameRu = service.Name.Ru,
            NameEn = service.Name.En,
            DescriptionRu = service.Description.Ru,
            DescriptionEn = service.Description.En,
            BasePrice = service.BasePrice,
            Unit = service.Unit,
            MinArea = service.MinArea,
            DurationMinutes = service.DurationMinutes,
            SortOrder = service.SortOrder,
            IsActive = service.IsActive
        };
    }
}