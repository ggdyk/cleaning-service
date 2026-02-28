using Api.Controllers.Abstractions;
using Application.DTOs.Calculator;
using Application.Features.Calculator.CalculatePrice;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Калькулятор стоимости уборки.
/// </summary>
[Route("api/calculator")]
public class CalculatorController : BaseController
{
    private readonly IMediator _mediator;

    public CalculatorController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Рассчитать стоимость уборки.
    /// </summary>
    /// <remarks>
    /// Публичный endpoint — не требует аутентификации.
    /// Формула: (площадь × цена_за_кв.м) + (санузлы × цена_за_санузел) + услуги + доп.услуги
    /// Итог не меньше минимальной суммы заказа для города.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(CalculatePriceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Calculate(
        [FromBody] CalculatePriceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CalculatePriceQuery(request), cancellationToken);
        return Ok(result);
    }
}
