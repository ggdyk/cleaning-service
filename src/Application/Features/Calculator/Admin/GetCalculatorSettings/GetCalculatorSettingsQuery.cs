using Application.DTOs.CalculatorSettings;
using MediatR;

namespace Application.Features.Calculator.Admin.GetCalculatorSettings;

public record GetCalculatorSettingsQuery : IRequest<IReadOnlyList<CalculatorSettingsResponse>>;
