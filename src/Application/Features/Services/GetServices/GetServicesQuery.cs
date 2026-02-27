using Application.DTOs.Services;
using MediatR;

namespace Application.Features.Services.GetServices;

/// <summary>
/// Запрос списка услуг. onlyActive=true - только активные (для клиентов).
/// </summary>
public record GetServicesQuery(bool OnlyActive = true) : IRequest<List<ServiceDto>>;
