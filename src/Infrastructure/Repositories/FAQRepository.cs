using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class FAQRepository : IFAQRepository
{
    private readonly ApplicationDbContext _context;

    public FAQRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<FAQ>> GetActiveAsync()
    {
        return await _context.FAQs
            .Where(f => f.IsActive)
            .OrderBy(f => f.SortOrder)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<FAQ>> GetAllAsync()
    {
        return await _context.FAQs
            .OrderBy(f => f.SortOrder)
            .ToListAsync();
    }

    public async Task<FAQ?> GetByIdAsync(int id)
    {
        return await _context.FAQs.FindAsync(id);
    }

    public async Task AddAsync(FAQ faq)
    {
        await _context.FAQs.AddAsync(faq);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(FAQ faq)
    {
        _context.FAQs.Update(faq);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(FAQ faq)
    {
        _context.FAQs.Remove(faq);
        await _context.SaveChangesAsync();
    }
}
