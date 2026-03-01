using Application.DTOs.ExtraServices;
using MediatR;

namespace Application.Features.ExtraServices.UpdateExtraService;

public record UpdateExtraServiceCommand(int Id, UpdateExtraServiceRequest Request) : IRequest<ExtraServiceDto>;
