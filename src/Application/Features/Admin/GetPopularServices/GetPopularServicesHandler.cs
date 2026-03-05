using Application.DTOs.Admin;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Admin.GetPopularServices;

public class GetPopularServicesHandler
    : IRequestHandler<GetPopularServicesQuery, IReadOnlyList<PopularServiceDto>>
{
    private readonly IAnalyticsRepository _analyticsRepository;

    public GetPopularServicesHandler(IAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    public Task<IReadOnlyList<PopularServiceDto>> Handle(
        GetPopularServicesQuery request,
        CancellationToken cancellationToken)
    {
        return _analyticsRepository.GetPopularServicesAsync(request.From, request.To, request.Top);
    }
}
