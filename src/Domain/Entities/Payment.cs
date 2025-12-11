using Domain.Common;

namespace Domain.Entities;

public class Payment : BaseEntity
{
    public int OrderId { get; set; } // Идентификатор заказа;
    public decimal Amount { get; set; } // Сумма платежа;
    public string Status { get; set; } // Статус платежа;
    public string PaymentMethod { get; set; } // Способ оплаты;
    public string TransactionId { get; set; } // Индификатор транзакций;
    public DateTime CreatedAt { get; set; } = DateTime.Now; // Дата и время создания платежа
    public DateTime? UpdatedAt { get; set; } // Дата и время последнего обновления
}