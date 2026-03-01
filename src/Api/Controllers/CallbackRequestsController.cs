using Application.DTOs.Callbacks;
using Application.Features.Callbacks.SubmitCallbackRequest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubmitCallbackRequestDto = Application.DTOs.Callbacks.SubmitCallbackRequest;

namespace Api.Controllers;

[ApiController]
[Route("api/callbacks")]
public class CallbackRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CallbackRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Оставить заявку на обратный звонок. Публичный доступ.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<CallbackRequestResponse>> Submit([FromBody] SubmitCallbackRequestDto request)
    {
        var result = await _mediator.Send(new SubmitCallbackRequestCommand(request));
        return StatusCode(201, result);
    }
}
