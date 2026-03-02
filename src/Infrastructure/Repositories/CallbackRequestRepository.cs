using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class CallbackRequestRepository : ICallbackRequestRepository
{
    private readonly ApplicationDbContext _context;

    public CallbackRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CallBackRequest request)
    {
        await _context.CallbackRequests.AddAsync(request);
        await _context.SaveChangesAsync();
    }
}
