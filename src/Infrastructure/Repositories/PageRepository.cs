using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PageRepository : IPageRepository
{
    private readonly ApplicationDbContext _context;

    public PageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Page?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        return await _context.Pages
            .Where(p => p.Slug == slug && p.IsActive)
            .FirstOrDefaultAsync(ct);
    }
}
