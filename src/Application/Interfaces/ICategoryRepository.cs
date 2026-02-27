using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
/// Репозиторий для работы с категориями услуг
/// </summary>
public interface ICategoryRepository
{
    /// <summary>Получить все категории. onlyActive=true — только активные.</summary>
    Task<List<Category>> GetAllAsync(bool onlyActive, CancellationToken cancellationToken = default);

    /// <summary>Получить категорию по Id.</summary>
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Добавить новую категорию.</summary>
    Task AddAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>Сохранить изменения категории.</summary>
    Task UpdateAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>Удалить категорию.</summary>
    Task DeleteAsync(Category category, CancellationToken cancellationToken = default);
}
