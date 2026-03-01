using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
/// Репозиторий дополнительных услуг.
/// </summary>
public interface IExtraServiceRepository
{
    /// <summary>Получить все активные дополнительные услуги.</summary>
    Task<List<ExtraService>> GetAllActiveAsync(CancellationToken ct = default);

    /// <summary>Получить все дополнительные услуги (включая неактивные).</summary>
    Task<List<ExtraService>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Получить дополнительную услугу по ID.</summary>
    Task<ExtraService?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Получить дополнительные услуги по списку ID.</summary>
    Task<List<ExtraService>> GetByIdsAsync(List<int> ids, CancellationToken ct = default);

    /// <summary>Добавить новую дополнительную услугу.</summary>
    Task AddAsync(ExtraService extraService, CancellationToken ct = default);

    /// <summary>Обновить существующую дополнительную услугу.</summary>
    Task UpdateAsync(ExtraService extraService, CancellationToken ct = default);

    /// <summary>Удалить дополнительную услугу.</summary>
    Task DeleteAsync(ExtraService extraService, CancellationToken ct = default);
}
