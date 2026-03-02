using Domain.Entities;

namespace Application.Interfaces;

public interface IPageRepository
{
    /// <summary>Найти активную страницу по slug.</summary>
    Task<Page?> GetBySlugAsync(string slug, CancellationToken ct = default);
}
