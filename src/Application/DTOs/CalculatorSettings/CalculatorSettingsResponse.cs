namespace Application.DTOs.CalculatorSettings;

public class CalculatorSettingsResponse
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public decimal PricePerSquareMeter { get; set; }
    public decimal PricePerBathroom { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    public DateTime UpdatedAt { get; set; }
}
