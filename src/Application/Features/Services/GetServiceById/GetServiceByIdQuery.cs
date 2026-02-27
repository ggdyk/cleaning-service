using Application.DTOs.Services;
using MediatR;

namespace Application.Features.Services.GetServicesById;

/// <summary>
/// Запрос конкретной услуги по Id.
/// </summary>
public record GetServiceByIdQuery(int Id) : IRequest<ServiceDto>;