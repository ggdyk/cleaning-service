using Application.DTOs.Services;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Services.GetServices;

public class GetServicesHandler : IRequestHandler<GetServicesQuery, List<ServiceDto>>
{
    private readonly IServiceRepository _serviceRepository;

    public GetServicesHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<List<ServiceDto>> Handle(GetServicesQuery query, CancellationToken cancellationToken)
    {
        var services = await _serviceRepository.GetAllAsync(query.OnlyActive, cancellationToken);
        
        //Маппим Entity в DTO прямо здесь, без AutoMapper (проще для интерна)
        return services.Select(s => new ServiceDto
        {
            Id = s.Id,
            CategoryId = s.CategoryId,
            NameRu = s.Name.Ru,
            NameEn = s.Name.En,
            DescriptionRu = s.Description.Ru,
            DescriptionEn = s.Description.En,
            BasePrice = s.BasePrice,
            Unit = s.Unit,
            MinArea = s.MinArea,
            DurationMinutes = s.DurationMinutes,
            SortOrder = s.SortOrder,
            IsActive = s.IsActive
        }).ToList();
    }
}