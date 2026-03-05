using Application.DTOs.Callbacks;
using Application.Interfaces;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Callbacks.Admin.ChangeCallbackStatus;

public class ChangeCallbackStatusHandler
    : IRequestHandler<ChangeCallbackStatusCommand, CallbackRequestResponse>
{
    private readonly ICallbackRequestRepository _repository;

    public ChangeCallbackStatusHandler(ICallbackRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<CallbackRequestResponse> Handle(
        ChangeCallbackStatusCommand command,
        CancellationToken cancellationToken)
    {
        var request = await _repository.GetByIdAsync(command.RequestId, cancellationToken)
            ?? throw new NotFoundException("CallbackRequest", command.RequestId);

        switch (command.NewStatus)
        {
            case CallbackRequestStatus.Processed:
                request.Process();
                break;

            case CallbackRequestStatus.Rejected:
                request.Reject();
                break;

            default:
                throw new BusinessRuleException(
                    $"Переход в статус '{command.NewStatus}' не поддерживается.");
        }

        await _repository.UpdateAsync(request, cancellationToken);

        return new CallbackRequestResponse
        {
            Id            = request.Id,
            Name          = request.Name,
            Phone         = request.Phone,
            PreferredTime = request.PreferredTime,
            Status        = request.Status.ToString(),
            CreatedAt     = request.CreatedAt
        };
    }
}
