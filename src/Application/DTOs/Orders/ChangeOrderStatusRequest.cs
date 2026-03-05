using Domain.Enums;

namespace Application.DTOs.Orders;

/// <summary>
/// Запрос на смену статуса заказа администратором.
/// </summary>
public class ChangeOrderStatusRequest
{
    /// <summary>
    /// Целевой статус. Допустимые переходы:
    /// New → Assigned | Cancelled
    /// Assigned → InProgress | Cancelled
    /// InProgress → Completed
    /// </summary>
    public OrderStatus Status { get; init; }

    /// <summary>ID уборщика. Обязателен при переходе в статус Assigned.</summary>
    public int? CleanerId { get; init; }

    /// <summary>Комментарий. Рекомендуется при отмене заказа.</summary>
    public string? Comment { get; init; }
}
