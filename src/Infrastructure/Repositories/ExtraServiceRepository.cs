using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ExtraServiceRepository : IExtraServiceRepository
{
    private readonly ApplicationDbContext _db;

    public ExtraServiceRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<List<ExtraService>> GetAllActiveAsync(CancellationToken ct = default)
        => _db.ExtraServices
              .AsNoTracking()
              .Where(e => e.IsActive)
              .OrderBy(e => e.Name)
              .ToListAsync(ct);

    public Task<List<ExtraService>> GetAllAsync(CancellationToken ct = default)
        => _db.ExtraServices
              .AsNoTracking()
              .OrderBy(e => e.Name)
              .ToListAsync(ct);

    public Task<ExtraService?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.ExtraServices.FindAsync([id], ct).AsTask();

    public Task<List<ExtraService>> GetByIdsAsync(List<int> ids, CancellationToken ct = default)
        => _db.ExtraServices
              .AsNoTracking()
              .Where(e => ids.Contains(e.Id))
              .ToListAsync(ct);

    public async Task AddAsync(ExtraService extraService, CancellationToken ct = default)
    {
        await _db.ExtraServices.AddAsync(extraService, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(ExtraService extraService, CancellationToken ct = default)
    {
        _db.ExtraServices.Update(extraService);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(ExtraService extraService, CancellationToken ct = default)
    {
        _db.ExtraServices.Remove(extraService);
        await _db.SaveChangesAsync(ct);
    }
}
