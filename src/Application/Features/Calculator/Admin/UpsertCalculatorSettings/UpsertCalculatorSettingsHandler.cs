using Application.DTOs.CalculatorSettings;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Calculator.Admin.UpsertCalculatorSettings;

public class UpsertCalculatorSettingsHandler
    : IRequestHandler<UpsertCalculatorSettingsCommand, CalculatorSettingsResponse>
{
    private readonly ICalculatorSettingsRepository _repository;

    public UpsertCalculatorSettingsHandler(ICalculatorSettingsRepository repository)
    {
        _repository = repository;
    }

    public async Task<CalculatorSettingsResponse> Handle(
        UpsertCalculatorSettingsCommand command,
        CancellationToken cancellationToken)
    {
        var req = command.Request;

        // Ищем существующие настройки для города (с отслеживанием EF)
        var settings = await _repository.FindByCityIdAsync(req.CityId, cancellationToken);

        if (settings is null)
        {
            // Первый раз для этого города — создаём через Domain-фабрику
            settings = CalculatorSettings.Create(
                req.CityId,
                req.PricePerSquareMeter,
                req.PricePerBathroom,
                req.MinimumOrderAmount);

            await _repository.AddAsync(settings, cancellationToken);
        }
        else
        {
            // Обновляем через Domain-метод (бизнес-правила валидируются внутри)
            settings.Update(
                req.PricePerSquareMeter,
                req.PricePerBathroom,
                req.MinimumOrderAmount);
        }

        await _repository.SaveAsync(cancellationToken);

        return new CalculatorSettingsResponse
        {
            Id                  = settings.Id,
            CityId              = settings.CityId,
            PricePerSquareMeter = settings.PricePerSquareMeter,
            PricePerBathroom    = settings.PricePerBathroom,
            MinimumOrderAmount  = settings.MinimumOrderAmount,
            UpdatedAt           = settings.UpdatedAt
        };
    }
}
