namespace Domain.Entities;

public class OrderStatusHistory
{
    public int Id { get; set; } // Уникальный индификатор записи;
    public int OrderId { get; set; } // Индфикатор заказа;
    public string PreviousStatus { get; set; } // Предыдущий статус заказа;
    public string NewStatus { get; set; } // Новый статус заказа;
    public DateTime ChangedAt { get; set; } = DateTime.Now; // Дата и время изменения статуса;
    public int UserId { get; set; } // Индификатор сотрудника, который изменил статус;
    public string Comment { get; set; } // Комментарий к изменению статуса
}