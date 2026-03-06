using Application.DTOs.Callbacks;
using Application.Features.Callbacks.SubmitCallbackRequest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubmitCallbackRequestDto = Application.DTOs.Callbacks.SubmitCallbackRequest;

namespace Api.Controllers;

/// <summary>
/// Заявки на обратный звонок от потенциальных клиентов.
/// </summary>
[ApiController]
[Route("api/callbacks")]
[Produces("application/json")]
public class CallbackRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CallbackRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Оставить заявку на обратный звонок
    /// </summary>
    /// <remarks>
    /// Публичный endpoint — авторизация не требуется.
    /// Заявка получает статус New и появляется в административной панели
    /// (`GET /api/admin/callbacks`).
    ///
    /// Пример запроса:
    ///
    ///     POST /api/callbacks
    ///     {
    ///         "name": "Анна",
    ///         "phone": "+77771234567",
    ///         "preferredTime": "С 10:00 до 12:00"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Заявка принята</response>
    /// <response code="400">Ошибка валидации (имя или телефон не заполнены)</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CallbackRequestResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CallbackRequestResponse>> Submit([FromBody] SubmitCallbackRequestDto request)
    {
        var result = await _mediator.Send(new SubmitCallbackRequestCommand(request));
        return StatusCode(201, result);
    }
}
