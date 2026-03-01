using Application.DTOs.ExtraServices;
using MediatR;

namespace Application.Features.ExtraServices.GetExtraServices;

/// <param name="OnlyActive">Если true — только активные (по умолчанию). Admin передаёт false чтобы видеть все.</param>
public record GetExtraServicesQuery(bool OnlyActive = true) : IRequest<List<ExtraServiceDto>>;
