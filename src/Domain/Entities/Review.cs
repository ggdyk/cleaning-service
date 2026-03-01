using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Отзыв клиента об оказанной услуге.
/// Перед публикацией проходит модерацию (Pending → Approved | Rejected).
/// </summary>
public class Review : BaseEntity
{
    /// <summary>ID пользователя (Identity context), оставившего отзыв.</summary>
    public int UserId { get; private set; }

    /// <summary>
    /// ID заказа (Orders context), к которому относится отзыв.
    /// Null — если отзыв оставлен без привязки к конкретному заказу.
    /// </summary>
    public int? OrderId { get; private set; }

    /// <summary>Отображаемое имя автора отзыва.</summary>
    public string AuthorName { get; private set; } = default!;

    /// <summary>Оценка от 1 (плохо) до 5 (отлично).</summary>
    public int Rating { get; private set; }

    /// <summary>Текст отзыва.</summary>
    public string ReviewText { get; private set; } = default!;

    /// <summary>Текущий статус модерации.</summary>
    public ReviewModerationStatus ModerationStatus { get; private set; }

    /// <summary>Дата и время создания (UTC).</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Дата и время принятия решения по модерации (UTC). Null — если ещё не рассмотрен.</summary>
    public DateTime? ModeratedAt { get; private set; }

    /// <summary>ID модератора (Identity context), принявшего решение. Null — если ещё не рассмотрен.</summary>
    public int? ModeratorId { get; private set; }

    // Конструктор для EF Core
    private Review() { }

    /// <summary>
    /// Создать новый отзыв. Статус модерации устанавливается в <see cref="ReviewModerationStatus.Pending"/>.
    /// </summary>
    /// <param name="userId">ID пользователя из Identity context.</param>
    /// <param name="authorName">Отображаемое имя автора.</param>
    /// <param name="rating">Оценка от 1 до 5.</param>
    /// <param name="reviewText">Текст отзыва.</param>
    /// <param name="orderId">ID заказа (опционально).</param>
    /// <exception cref="ArgumentException">Если рейтинг вне диапазона 1–5 или текст пуст.</exception>
    public static Review Create(
        int userId,
        string authorName,
        int rating,
        string reviewText,
        int? orderId = null)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Оценка должна быть от 1 до 5.", nameof(rating));

        if (string.IsNullOrWhiteSpace(authorName))
            throw new ArgumentException("Имя автора не может быть пустым.", nameof(authorName));

        if (string.IsNullOrWhiteSpace(reviewText))
            throw new ArgumentException("Текст отзыва не может быть пустым.", nameof(reviewText));

        return new Review
        {
            UserId = userId,
            OrderId = orderId,
            AuthorName = authorName.Trim(),
            Rating = rating,
            ReviewText = reviewText.Trim(),
            ModerationStatus = ReviewModerationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Одобрить отзыв. После одобрения он становится публичным.
    /// </summary>
    /// <param name="moderatorId">ID модератора, принявшего решение.</param>
    /// <exception cref="InvalidOperationException">Если отзыв уже рассмотрен.</exception>
    public void Approve(int moderatorId)
    {
        if (ModerationStatus != ReviewModerationStatus.Pending)
            throw new InvalidOperationException(
                $"Невозможно одобрить отзыв в статусе '{ModerationStatus}'.");

        ModerationStatus = ReviewModerationStatus.Approved;
        ModeratorId = moderatorId;
        ModeratedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Отклонить отзыв. Отклонённый отзыв не отображается на сайте.
    /// </summary>
    /// <param name="moderatorId">ID модератора, принявшего решение.</param>
    /// <exception cref="InvalidOperationException">Если отзыв уже рассмотрен.</exception>
    public void Reject(int moderatorId)
    {
        if (ModerationStatus != ReviewModerationStatus.Pending)
            throw new InvalidOperationException(
                $"Невозможно отклонить отзыв в статусе '{ModerationStatus}'.");

        ModerationStatus = ReviewModerationStatus.Rejected;
        ModeratorId = moderatorId;
        ModeratedAt = DateTime.UtcNow;
    }
}
