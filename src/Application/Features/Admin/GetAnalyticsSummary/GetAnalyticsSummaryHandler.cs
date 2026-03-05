using Application.DTOs.Admin;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Admin.GetAnalyticsSummary;

public class GetAnalyticsSummaryHandler : IRequestHandler<GetAnalyticsSummaryQuery, AnalyticsSummaryDto>
{
    private readonly IAnalyticsRepository _analyticsRepository;

    public GetAnalyticsSummaryHandler(IAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    public Task<AnalyticsSummaryDto> Handle(GetAnalyticsSummaryQuery request, CancellationToken cancellationToken)
    {
        return _analyticsRepository.GetSummaryAsync();
    }
}
