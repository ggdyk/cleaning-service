using Domain.Common;

namespace Domain.Entities;

public class CalculatorSettings : BaseEntity
{
    public int CityId { get; set; } // Идентификатор города, для которого действуют эти настройки
    public decimal PricePerSquareMeter { get; set; } // Коэффициент (цена) за один квадратный метр
    public decimal PricePerBathroom { get; set; } // Коэффициент (цена) за один санузел
    public decimal MinimumOrderAmount { get; set; } // Минимальная сумма заказа
    public DateTime UpdatedAt { get; set; } = DateTime.Now; // Дата и время последнего обновления настроек
}