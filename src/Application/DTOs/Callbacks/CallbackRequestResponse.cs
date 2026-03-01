namespace Application.DTOs.Callbacks;

public class CallbackRequestResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string? PreferredTime { get; set; }
    public string Status { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
