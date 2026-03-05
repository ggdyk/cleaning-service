namespace Application.DTOs.Orders;

/// <summary>
/// Краткая информация о заказе для административного списка.
/// Включает ClientId и CleanerId — поля, недоступные в клиентском DTO.
/// </summary>
public class AdminOrderSummaryResponse
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ClientId { get; set; }
    public int? CleanerId { get; set; }
}
