using Api.Controllers.Abstractions;
using Application.DTOs.Calculator;
using Application.Features.Calculator.CalculatePrice;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Калькулятор предварительной стоимости уборки.
/// </summary>
[Route("api/calculator")]
[Produces("application/json")]
public class CalculatorController : BaseController
{
    private readonly IMediator _mediator;

    public CalculatorController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Рассчитать стоимость уборки
    /// </summary>
    /// <remarks>
    /// Публичный endpoint — авторизация не требуется.
    ///
    /// **Формула расчёта:**
    /// `итог = (площадь × цена_за_кв.м) + (санузлы × цена_за_санузел) + стоимость_услуги + сумма_доп.услуг`
    ///
    /// Итоговая сумма не может быть меньше минимальной суммы заказа для выбранного города.
    /// Коэффициенты для каждого города настраиваются через `PUT /api/admin/calculator-settings`.
    ///
    /// Пример запроса:
    ///
    ///     POST /api/calculator
    ///     {
    ///         "cityId": 1,
    ///         "area": 65.5,
    ///         "bathrooms": 1,
    ///         "serviceId": 2,
    ///         "extraServiceIds": [3, 7]
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Детализированный расчёт стоимости</response>
    /// <response code="400">Ошибка валидации (площадь ≤ 0, санузлы &lt; 0)</response>
    /// <response code="404">Город или услуга не найдены</response>
    /// <response code="422">Для выбранного города не настроены коэффициенты</response>
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
