using Application.DTOs.Callbacks;
using Application.Interfaces;
using MediatR;
using CallBackRequestEntity = Domain.Entities.CallBackRequest;

namespace Application.Features.Callbacks.SubmitCallbackRequest;

public class SubmitCallbackRequestHandler : IRequestHandler<SubmitCallbackRequestCommand, CallbackRequestResponse>
{
    private readonly ICallbackRequestRepository _repository;

    public SubmitCallbackRequestHandler(ICallbackRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<CallbackRequestResponse> Handle(
        SubmitCallbackRequestCommand command,
        CancellationToken cancellationToken)
    {
        var req = command.Request;

        var callbackRequest = CallBackRequestEntity.Create(
            name: req.Name,
            phone: req.Phone,
            preferredTime: req.PreferredTime);

        await _repository.AddAsync(callbackRequest);

        return new CallbackRequestResponse
        {
            Id = callbackRequest.Id,
            Name = callbackRequest.Name,
            Phone = callbackRequest.Phone,
            PreferredTime = callbackRequest.PreferredTime,
            Status = callbackRequest.Status.ToString(),
            CreatedAt = callbackRequest.CreatedAt
        };
    }
}
