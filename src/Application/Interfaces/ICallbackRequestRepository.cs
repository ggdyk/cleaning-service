using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces;

public interface ICallbackRequestRepository
{
    Task AddAsync(CallBackRequest request);

    Task<IReadOnlyList<CallBackRequest>> GetAllAsync(
        CallbackRequestStatus? status = null,
        CancellationToken ct = default);

    Task<CallBackRequest?> GetByIdAsync(int id, CancellationToken ct = default);

    Task UpdateAsync(CallBackRequest request, CancellationToken ct = default);
}
