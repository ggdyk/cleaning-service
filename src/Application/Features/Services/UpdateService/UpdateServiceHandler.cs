using Application.DTOs.Services;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Services.UpdateService;

public class UpdateServiceHandler : IRequestHandler<UpdateServiceCommand, ServiceDto>
{
    private readonly IServiceRepository _serviceRepository;

    public UpdateServiceHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<ServiceDto> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.Id, cancellationToken);

        if (service is null)
            throw new NotFoundException("Услуга", request.Id);

        var req = request.Request;

        service.Update(
            req.CategoryId,
            req.NameRu,
            req.NameEn,
            req.DescriptionRu,
            req.DescriptionEn,
            req.BasePrice,
            req.Unit,
            req.MinArea,
            req.DurationMinutes,
            req.SortOrder,
            req.IsActive);

        await _serviceRepository.UpdateAsync(service, cancellationToken);

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
