namespace Application.DTOs.ExtraServices;

public class ExtraServiceDto
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string Description { get; init; } = default!;
    public decimal Price { get; init; }
    public string Unit { get; init; } = default!;
    public bool IsActive { get; init; }
}
