namespace Application.DTOs.Callbacks;

public class SubmitCallbackRequest
{
    /// <summary>Имя клиента.</summary>
    public string Name { get; set; } = default!;

    /// <summary>Контактный номер телефона.</summary>
    public string Phone { get; set; } = default!;

    /// <summary>Удобное время для звонка (необязательно).</summary>
    public string? PreferredTime { get; set; }
}
