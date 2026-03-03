namespace Domain.Enums;

/// <summary>
/// Статус модерации отзыва.
/// Допустимые переходы:
/// Pending → Approved
/// Pending → Rejected
/// </summary>
public enum ReviewModerationStatus
{
    /// <summary>Отзыв ожидает проверки модератором.</summary>
    Pending = 1,

    /// <summary>Отзыв одобрен и отображается на сайте.</summary>
    Approved = 2,

    /// <summary>Отзыв отклонён модератором — не отображается на сайте.</summary>
    Rejected = 3
}
