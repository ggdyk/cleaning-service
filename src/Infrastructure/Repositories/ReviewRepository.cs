using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Review>> GetApprovedAsync()
    {
        return await _context.Reviews
            .Where(r => r.ModerationStatus == ReviewModerationStatus.Approved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Review>> GetPendingAsync()
    {
        return await _context.Reviews
            .Where(r => r.ModerationStatus == ReviewModerationStatus.Pending)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Review>> GetAllAsync()
    {
        return await _context.Reviews
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Review?> GetByIdAsync(int id)
    {
        return await _context.Reviews.FindAsync(id);
    }

    public async Task AddAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Review review)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
    }
}
