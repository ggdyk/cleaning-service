namespace Application.DTOs.Admin;

/// <summary>
/// Одна точка на графике динамики заказов.
/// </summary>
/// <param name="Period">
/// Метка периода:
/// Day  → "2026-03-06",
/// Week → "2026-W10",
/// Month→ "2026-03"
/// </param>
/// <param name="Count">Количество заказов в этом периоде (0 если заказов не было).</param>
public record OrdersByPeriodItemDto(string Period, int Count);
