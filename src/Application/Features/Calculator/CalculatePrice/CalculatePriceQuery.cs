using Application.DTOs.Calculator;
using MediatR;

namespace Application.Features.Calculator.CalculatePrice;

public record CalculatePriceQuery(CalculatePriceRequest Request) : IRequest<CalculatePriceResponse>;
