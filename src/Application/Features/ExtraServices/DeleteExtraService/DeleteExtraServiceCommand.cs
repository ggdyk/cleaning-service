using MediatR;

namespace Application.Features.ExtraServices.DeleteExtraService;

public record DeleteExtraServiceCommand(int Id) : IRequest;
