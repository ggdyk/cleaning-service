using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CalculatorSettingsRepository : ICalculatorSettingsRepository
{
    private readonly ApplicationDbContext _db;

    public CalculatorSettingsRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<CalculatorSettings?> GetByCityIdAsync(int cityId, CancellationToken ct = default)
        => _db.CalculatorSettings
              .AsNoTracking()
              .FirstOrDefaultAsync(s => s.CityId == cityId, ct);

    public Task<CalculatorSettings?> GetDefaultAsync(CancellationToken ct = default)
        => _db.CalculatorSettings
              .AsNoTracking()
              .OrderBy(s => s.Id)
              .FirstOrDefaultAsync(ct);
}
