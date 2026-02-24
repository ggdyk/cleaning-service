namespace Domain.Enums;

/// <summary>
/// Статусы жизненного цикла заказа.
/// Допустимые переходы:
/// New → Assigned → InProgress → Completed
/// New → Cancelled
/// Assigned → Cancelled
/// </summary>
public enum OrderStatus
{
    /// <summary>Заказ создан, уборщик ещё не назначен.</summary>
    New = 1,

    /// <summary>Уборщик назначен, ожидает начала работы.</summary>
    Assigned = 2,

    /// <summary>Уборщик приступил к работе.</summary>
    InProgress = 3,

    /// <summary>Уборка завершена.</summary>
    Completed = 4,

    /// <summary>Заказ отменён клиентом или менеджером.</summary>
    Cancelled = 5
}