using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces;

public interface IReviewRepository
{
    Task<IReadOnlyList<Review>> GetApprovedAsync();
    Task<IReadOnlyList<Review>> GetPendingAsync();
    Task<IReadOnlyList<Review>> GetAllAsync();
    Task<Review?> GetByIdAsync(int id);
    Task AddAsync(Review review);
    Task UpdateAsync(Review review);
}
