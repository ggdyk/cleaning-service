using Application.DTOs.CalculatorSettings;
using MediatR;

namespace Application.Features.Calculator.Admin.UpsertCalculatorSettings;

public record UpsertCalculatorSettingsCommand(
    UpsertCalculatorSettingsRequest Request
) : IRequest<CalculatorSettingsResponse>;
