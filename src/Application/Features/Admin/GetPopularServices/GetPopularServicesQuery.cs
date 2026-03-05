using Application.DTOs.Admin;
using MediatR;

namespace Application.Features.Admin.GetPopularServices;

public record GetPopularServicesQuery(
    DateTime? From,
    DateTime? To,
    int Top = 10
) : IRequest<IReadOnlyList<PopularServiceDto>>;
