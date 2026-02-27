using Application.DTOs.Services;
using MediatR;

namespace Application.Features.Services.UpdateService;

/// <summary>
/// Команда обновления услуги (только Admin).
/// </summary>
public record UpdateServiceCommand(int Id, UpdateServiceRequest Request) : IRequest<ServiceDto>;
