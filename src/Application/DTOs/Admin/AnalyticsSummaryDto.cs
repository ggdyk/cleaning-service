namespace Application.DTOs.Admin;

public record AnalyticsSummaryDto(
    int TotalOrders,
    int OrdersThisMonth,
    int OrdersThisWeek,
    int TotalUsers,
    int TotalCallbackRequests
);
