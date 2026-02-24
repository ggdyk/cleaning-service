using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Services)
            .Include(o => o.ExtraServices)
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IReadOnlyList<Order>> GetByClientIdAsync(int clientId)
    {
        return await _context.Orders
            .Where(o => o.ClientId == clientId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }
}
