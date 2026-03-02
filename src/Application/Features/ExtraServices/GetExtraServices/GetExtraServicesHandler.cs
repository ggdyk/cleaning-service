using Application.DTOs.ExtraServices;
using Application.Interfaces;
using MediatR;

namespace Application.Features.ExtraServices.GetExtraServices;

public class GetExtraServicesHandler : IRequestHandler<GetExtraServicesQuery, List<ExtraServiceDto>>
{
    private readonly IExtraServiceRepository _repository;

    public GetExtraServicesHandler(IExtraServiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ExtraServiceDto>> Handle(GetExtraServicesQuery query, CancellationToken cancellationToken)
    {
        var items = query.OnlyActive
            ? await _repository.GetAllActiveAsync(cancellationToken)
            : await _repository.GetAllAsync(cancellationToken);

        return items.Select(ToDto).ToList();
    }

    internal static ExtraServiceDto ToDto(Domain.Entities.ExtraService e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Description = e.Description,
        Price = e.Price,
        Unit = e.Unit,
        IsActive = e.IsActive
    };
}
