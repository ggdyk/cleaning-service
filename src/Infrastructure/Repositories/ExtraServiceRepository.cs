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

    public Task<List<ExtraService>> GetByIdsAsync(List<int> ids, CancellationToken ct = default)
        => _db.ExtraServices
              .AsNoTracking()
              .Where(e => ids.Contains(e.Id))
              .ToListAsync(ct);

    public Task<List<ExtraService>> GetAllActiveAsync(CancellationToken ct = default)
        => _db.ExtraServices
              .AsNoTracking()
              .Where(e => e.IsActive)
              .ToListAsync(ct);
}
