using Domain.Entities;

namespace Application.Interfaces;

public interface IFAQRepository
{
    Task<IReadOnlyList<FAQ>> GetActiveAsync();
    Task<IReadOnlyList<FAQ>> GetAllAsync();
    Task<FAQ?> GetByIdAsync(int id);
    Task AddAsync(FAQ faq);
    Task UpdateAsync(FAQ faq);
    Task DeleteAsync(FAQ faq);
}
