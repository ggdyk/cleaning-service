using Application.DTOs.Callbacks;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Callbacks.Admin.GetAllCallbacks;

public class GetAllCallbacksHandler
    : IRequestHandler<GetAllCallbacksQuery, IReadOnlyList<CallbackRequestResponse>>
{
    private readonly ICallbackRequestRepository _repository;

    public GetAllCallbacksHandler(ICallbackRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CallbackRequestResponse>> Handle(
        GetAllCallbacksQuery query,
        CancellationToken cancellationToken)
    {
        var requests = await _repository.GetAllAsync(query.Status, cancellationToken);

        return requests.Select(r => new CallbackRequestResponse
        {
            Id            = r.Id,
            Name          = r.Name,
            Phone         = r.Phone,
            PreferredTime = r.PreferredTime,
            Status        = r.Status.ToString(),
            CreatedAt     = r.CreatedAt
        }).ToList();
    }
}
