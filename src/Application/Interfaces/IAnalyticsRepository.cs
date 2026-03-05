using Application.DTOs.Admin;

namespace Application.Interfaces;

public interface IAnalyticsRepository
{
    Task<AnalyticsSummaryDto> GetSummaryAsync();

    Task<IReadOnlyList<PopularServiceDto>> GetPopularServicesAsync(
        DateTime? from,
        DateTime? to,
        int top);

    Task<IReadOnlyList<OrdersByPeriodItemDto>> GetOrdersByPeriodAsync(
        DateTime startDate,
        DateTime endDate,
        GroupByPeriod groupBy);
}
