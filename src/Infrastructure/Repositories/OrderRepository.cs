using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
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

    public async Task<IReadOnlyList<Order>> GetAllAsync(
        OrderStatus? status = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        int? clientId = null,
        CancellationToken ct = default)
    {
        var query = _context.Orders.AsQueryable();

        if (status.HasValue)    query = query.Where(o => o.Status == status.Value);
        if (dateFrom.HasValue)  query = query.Where(o => o.CreatedAt >= dateFrom.Value);
        if (dateTo.HasValue)    query = query.Where(o => o.CreatedAt <= dateTo.Value);
        if (clientId.HasValue)  query = query.Where(o => o.ClientId == clientId.Value);

        return await query
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        // Заказ уже отслеживается EF (загружен через GetByIdAsync без AsNoTracking),
        // достаточно сохранить изменения.
        await _context.SaveChangesAsync();
    }
}
