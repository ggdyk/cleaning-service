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

/// <summary>
/// Управление услугами клининга. Чтение — публичное. Запись — только Admin.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
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
    /// <response code="200">Список услуг</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ServiceDto>), StatusCodes.Status200OK)]
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
    /// <param name="id">ID услуги</param>
    /// <response code="200">Данные услуги</response>
    /// <response code="404">Услуга не найдена</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// <response code="201">Услуга создана</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав (требуется роль Admin)</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    /// <param name="id">ID услуги</param>
    /// <param name="request">Новые данные услуги</param>
    /// <response code="200">Обновлённые данные услуги</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав (требуется роль Admin)</response>
    /// <response code="404">Услуга не найдена</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// <param name="id">ID услуги</param>
    /// <response code="204">Услуга удалена</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав (требуется роль Admin)</response>
    /// <response code="404">Услуга не найдена</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(
        [FromRoute] int id)
    {
        var command = new DeleteServiceCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}
