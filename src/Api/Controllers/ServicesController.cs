using Application.DTOs.Services;
using Application.Features.Services.CreateService;
using Application.Features.Services.DeleteService;
using Application.Features.Services.GetServices;
using Application.Features.Services.GetServicesById;
using Application.Features.Services.UpdateService;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Получить список услуг
    /// </summary>
    /// <param name="onlyActive">Если true — только активные услуги (по умолчанию true)</param>
    [HttpGet]
    public async Task<ActionResult<List<ServiceDto>>> GetAll(
        [FromQuery] bool onlyActive = true)
    {
        var query = new GetServicesQuery(onlyActive);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Получить услугу по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceDto>> GetById(
        [FromRoute] int id)
    {
        var query = new GetServiceByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Создать услугу (только Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceDto>> Create(
        [FromBody] CreateServiceRequest request)
    {
        var command = new CreateServiceCommand(request);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Обновить услугу (только Admin)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceDto>> Update(
        [FromRoute] int id,
        [FromBody] UpdateServiceRequest request)
    {
        var command = new UpdateServiceCommand(id, request);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Удалить услугу (только Admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(
        [FromRoute] int id)
    {
        var command = new DeleteServiceCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}
