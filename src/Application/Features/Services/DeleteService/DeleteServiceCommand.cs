using MediatR;

namespace Application.Features.Services.DeleteService;

/// <summary>
/// Команда удаления услуги (только Admin).
/// </summary>
public record DeleteServiceCommand(int Id) : IRequest;
