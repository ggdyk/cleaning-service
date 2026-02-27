using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
/// Репозиторий для работы с услугами 
/// </summary>
public interface IServiceRepository
{

    /// <summary>
    /// Получить все услуги. onlyActive=true - только активные. 
    /// </summary>
    Task<List<Service>> GetAllAsync(bool onlyActive, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить услугу по Id.
    /// </summary>
    Task<Service?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить новую услугу.
    /// </summary>
    Task AddAsync(Service service, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохранить изменения услуги.
    /// </summary>
    Task UpdateAsync(Service service, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить услугу.
    /// </summary>
    Task DeleteAsync(Service service, CancellationToken cancellationToken = default);
}