using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IReadOnlyList<CallBackRequest>> GetAllAsync(
        CallbackRequestStatus? status = null,
        CancellationToken ct = default)
    {
        var query = _context.CallbackRequests.AsQueryable();

        if (status.HasValue) query = query.Where(r => r.Status == status.Value);

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<CallBackRequest?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.CallbackRequests.FindAsync([id], ct);

    public async Task UpdateAsync(CallBackRequest request, CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
