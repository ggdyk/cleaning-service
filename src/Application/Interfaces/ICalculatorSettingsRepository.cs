using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
/// Репозиторий настроек калькулятора цен.
/// </summary>
public interface ICalculatorSettingsRepository
{
    /// <summary>
    /// Получить настройки для указанного города (только чтение, без отслеживания).
    /// Используется калькулятором цен.
    /// </summary>
    Task<CalculatorSettings?> GetByCityIdAsync(int cityId, CancellationToken ct = default);

    /// <summary>
    /// Получить настройки по умолчанию (первая запись). Fallback для калькулятора.
    /// </summary>
    Task<CalculatorSettings?> GetDefaultAsync(CancellationToken ct = default);

    /// <summary>
    /// Получить все настройки (все города). Для административного просмотра.
    /// </summary>
    Task<IReadOnlyList<CalculatorSettings>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Найти настройки для города с отслеживанием (для последующего обновления).
    /// </summary>
    Task<CalculatorSettings?> FindByCityIdAsync(int cityId, CancellationToken ct = default);

    Task AddAsync(CalculatorSettings settings, CancellationToken ct = default);
    Task SaveAsync(CancellationToken ct = default);
}