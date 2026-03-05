using Application.DTOs.Callbacks;
using Domain.Enums;
using MediatR;

namespace Application.Features.Callbacks.Admin.ChangeCallbackStatus;

public record ChangeCallbackStatusCommand(
    int RequestId,
    CallbackRequestStatus NewStatus
) : IRequest<CallbackRequestResponse>;
