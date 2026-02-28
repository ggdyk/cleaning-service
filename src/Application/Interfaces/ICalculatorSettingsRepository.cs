using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
/// Репозиторий настроек калькулятора цен.
/// </summary>
public interface ICalculatorSettingsRepository
{
    /// <summary>
    /// Получить настройки калькулятора для указанного города.
    /// Возвращает <c>null</c>, если настройки не заданы.
    /// </summary>
    Task<CalculatorSettings?> GetByCityIdAsync(int cityId, CancellationToken ct = default);

    /// <summary>
    /// Получить настройки калькулятора по умолчанию (первая запись в БД).
    /// Используется как fallback, если для города нет отдельных настроек.
    /// </summary>
    Task<CalculatorSettings?> GetDefaultAsync(CancellationToken ct = default);
}