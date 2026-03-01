namespace Application.DTOs.ExtraServices;

public class CreateExtraServiceRequest
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public string Unit { get; set; } = default!;
}
