namespace Application.DTOs.Orders;

/// <summary>
/// Краткая информация о заказе — для списка заказов пользователя.
/// </summary>
public class OrderSummaryResponse
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
}
