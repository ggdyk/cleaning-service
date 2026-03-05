using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<IReadOnlyList<Order>> GetByClientIdAsync(int clientId);

    /// <summary>
    /// Все заказы с опциональными фильтрами — для администратора.
    /// </summary>
    Task<IReadOnlyList<Order>> GetAllAsync(
        OrderStatus? status = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        int? clientId = null,
        CancellationToken ct = default);

    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
}
