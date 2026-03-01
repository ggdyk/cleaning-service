using Application.DTOs.ExtraServices;
using MediatR;

namespace Application.Features.ExtraServices.CreateExtraService;

public record CreateExtraServiceCommand(CreateExtraServiceRequest Request) : IRequest<ExtraServiceDto>;
