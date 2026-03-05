using Application.DTOs.CalculatorSettings;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Calculator.Admin.GetCalculatorSettings;

public class GetCalculatorSettingsHandler
    : IRequestHandler<GetCalculatorSettingsQuery, IReadOnlyList<CalculatorSettingsResponse>>
{
    private readonly ICalculatorSettingsRepository _repository;

    public GetCalculatorSettingsHandler(ICalculatorSettingsRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CalculatorSettingsResponse>> Handle(
        GetCalculatorSettingsQuery request,
        CancellationToken cancellationToken)
    {
        var settings = await _repository.GetAllAsync(cancellationToken);

        return settings.Select(s => new CalculatorSettingsResponse
        {
            Id                  = s.Id,
            CityId              = s.CityId,
            PricePerSquareMeter = s.PricePerSquareMeter,
            PricePerBathroom    = s.PricePerBathroom,
            MinimumOrderAmount  = s.MinimumOrderAmount,
            UpdatedAt           = s.UpdatedAt
        }).ToList();
    }
}
