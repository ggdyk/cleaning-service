namespace Domain.Enums;

public enum CallbackRequestStatus
{
    /// <summary>Новая заявка, ещё не обработана.</summary>
    New = 0,

    /// <summary>Оператор позвонил клиенту — заявка закрыта.</summary>
    Processed = 1,

    /// <summary>Заявка отклонена (неверный номер, отказ и т.д.).</summary>
    Rejected = 2
}
