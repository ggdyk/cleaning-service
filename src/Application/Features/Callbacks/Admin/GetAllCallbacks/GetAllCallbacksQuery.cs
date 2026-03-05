using Application.DTOs.Callbacks;
using Domain.Enums;
using MediatR;

namespace Application.Features.Callbacks.Admin.GetAllCallbacks;

public record GetAllCallbacksQuery(
    CallbackRequestStatus? Status
) : IRequest<IReadOnlyList<CallbackRequestResponse>>;
