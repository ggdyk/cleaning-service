using Application.DTOs.Admin;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Admin.GetOrdersByPeriod;

public class GetOrdersByPeriodHandler
    : IRequestHandler<GetOrdersByPeriodQuery, IReadOnlyList<OrdersByPeriodItemDto>>
{
    private readonly IAnalyticsRepository _analyticsRepository;

    public GetOrdersByPeriodHandler(IAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    public async Task<IReadOnlyList<OrdersByPeriodItemDto>> Handle(
        GetOrdersByPeriodQuery request,
        CancellationToken cancellationToken)
    {
        if (request.EndDate < request.StartDate)
            throw new BusinessRuleException("endDate не может быть раньше startDate.");

        return await _analyticsRepository.GetOrdersByPeriodAsync(
            request.StartDate,
            request.EndDate,
            request.GroupBy);
    }
}
