namespace Domain.Entities;

public class CallBackRequest
{
    public int Id { get; set; } // Уникальный идентификатор заявки
    public string Name { get; set; } // Имя клиента, запросившего звонок 
    public string Phone { get; set; } // Номер телефона клиента 
    public string PreferredTime { get; set; } // Удобное для клиента время звонков
    public string Status { get; set; } // Текущий статус обработки заявки
    public string OperatorComment { get; set; } // Комментатор оператора 
    public DateTime CreatedAt { get; set; } = DateTime.Now; // Дата и время создания заявки
    public DateTime? ProcessedAt { get; set; } // Дата и время обработки заявки
}