using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
/// Репозиторий дополнительных услуг.
/// </summary>
public interface IExtraServiceRepository
{
    /// <summary>
    /// Получить дополнительные услуги по списку ID.
    /// </summary>
    Task<List<ExtraService>> GetByIdsAsync(List<int> ids, CancellationToken ct = default);

    /// <summary>
    /// Получить все активные дополнительные услуги.
    /// </summary>
    Task<List<ExtraService>> GetAllActiveAsync(CancellationToken ct = default);
}
