using Domain.Entities;

namespace Application.Interfaces;

public interface ICallbackRequestRepository
{
    Task AddAsync(CallBackRequest request);
}
