using Application.DTOs.Services;
using MediatR;

namespace Application.Features.Services.CreateService;

/// <summary>
/// Команда создания услуги (только Admin).
/// </summary>
public record CreateServiceCommand(CreateServiceRequest Request) : IRequest<ServiceDto>;