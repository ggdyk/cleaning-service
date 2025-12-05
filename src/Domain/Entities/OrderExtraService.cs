namespace Domain.Entities;

public class OrderExtraService
{
    public int Id { get; set; } // Уникальный индификатор 
    public int OrderId { get; set; } // Индификатор заказа
    public int ExtraServiceId { get; set; } // Индификатор доп. услуги. Ссылка на оригинальную услугу;
    public string Name { get; set; } // Название услуги 
    public decimal UnitPrice { get; set; } // Цена за одну единицу услуги;
    public int Quantity { get; set; } // Количество единиц услуг. Например, 2 мойки ковров;
    
    //Вычисляемое свойство 
    public decimal TotalPrice => UnitPrice * Quantity; 

    public DateTime CreatedAt { get; set; } = DateTime.Now; // Дата и время создания записи;
}