using Application.DTOs.ExtraServices;
using Application.Features.ExtraServices.CreateExtraService;
using Application.Features.ExtraServices.DeleteExtraService;
using Application.Features.ExtraServices.GetExtraServiceById;
using Application.Features.ExtraServices.GetExtraServices;
using Application.Features.ExtraServices.UpdateExtraService;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/extra-services")]
public class ExtraServicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExtraServicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Получить список активных дополнительных услуг. Публичный доступ.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<ExtraServiceDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetExtraServicesQuery(OnlyActive: true));
        return Ok(result);
    }

    /// <summary>
    /// Получить дополнительную услугу по ID. Публичный доступ.
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ExtraServiceDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetExtraServiceByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Создать дополнительную услугу. Только Admin.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ExtraServiceDto>> Create([FromBody] CreateExtraServiceRequest request)
    {
        var result = await _mediator.Send(new CreateExtraServiceCommand(request));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Обновить дополнительную услугу по ID. Только Admin.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ExtraServiceDto>> Update(int id, [FromBody] UpdateExtraServiceRequest request)
    {
        var result = await _mediator.Send(new UpdateExtraServiceCommand(id, request));
        return Ok(result);
    }

    /// <summary>
    /// Удалить дополнительную услугу по ID. Только Admin.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteExtraServiceCommand(id));
        return NoContent();
    }
}
