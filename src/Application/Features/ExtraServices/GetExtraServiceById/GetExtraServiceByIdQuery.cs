using Application.DTOs.ExtraServices;
using MediatR;

namespace Application.Features.ExtraServices.GetExtraServiceById;

public record GetExtraServiceByIdQuery(int Id) : IRequest<ExtraServiceDto>;
