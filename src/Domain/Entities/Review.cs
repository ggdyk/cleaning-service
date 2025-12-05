namespace Domain.Entities;

public class Review
{
    public int Id { get; set; } // Уникальный индификатор отзыва;
    public int UserId { get; set; } // Идентификатор пользователя;
    public int? OrderId { get; set; } // Индентификатор заказа, к которому относится отзыв;
    public string AuthorName { get; set; } // Отображаемое имя автора отзыва;
    public int Rating { get; set; } // Оценка в виде числа от 1 до 5;
    public string ReviewText { get; set; } // Текст отзыва (само содержание):
    public string ModerationStatus { get; set; } // Статус модерации отзыва:
    public DateTime CreatedAt { get; set; } = DateTime.Now; // Дата и время создания отзыва
    public DateTime? ModeratedAt { get; set; } // Дата и время модерации
    public int? ModeratorId { get; set; } // Индификатор модератора
}