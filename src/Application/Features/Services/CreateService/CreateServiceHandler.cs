using Application.DTOs.Services;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Services.CreateService;

public class CreateServiceHandler : IRequestHandler<CreateServiceCommand, ServiceDto>
{
    private readonly IServiceRepository _serviceRepository;

    public CreateServiceHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<ServiceDto> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        var service = Service.Create(
            req.CategoryId,
            req.NameRu,
            req.NameEn,
            req.DescriptionRu,
            req.DescriptionEn,
            req.BasePrice,
            req.Unit,
            req.MinArea,
            req.DurationMinutes,
            req.SortOrder);

        await _serviceRepository.AddAsync(service, cancellationToken);

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
