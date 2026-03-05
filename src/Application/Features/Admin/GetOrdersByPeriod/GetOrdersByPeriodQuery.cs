using Application.DTOs.Admin;
using MediatR;

namespace Application.Features.Admin.GetOrdersByPeriod;

public record GetOrdersByPeriodQuery(
    DateTime StartDate,
    DateTime EndDate,
    GroupByPeriod GroupBy
) : IRequest<IReadOnlyList<OrdersByPeriodItemDto>>;
