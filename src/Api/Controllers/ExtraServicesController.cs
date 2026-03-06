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

/// <summary>
/// Управление дополнительными услугами (уборка балкона, глажка и т.д.).
/// Чтение — публичное. Запись — только Admin.
/// </summary>
[ApiController]
[Route("api/extra-services")]
[Produces("application/json")]
public class ExtraServicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExtraServicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Получить список активных дополнительных услуг
    /// </summary>
    /// <remarks>Публичный endpoint, авторизация не требуется.</remarks>
    /// <response code="200">Список дополнительных услуг</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ExtraServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ExtraServiceDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetExtraServicesQuery(OnlyActive: true));
        return Ok(result);
    }

    /// <summary>
    /// Получить дополнительную услугу по ID
    /// </summary>
    /// <remarks>Публичный endpoint, авторизация не требуется.</remarks>
    /// <param name="id">ID дополнительной услуги</param>
    /// <response code="200">Данные услуги</response>
    /// <response code="404">Услуга не найдена</response>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ExtraServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExtraServiceDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetExtraServiceByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Создать дополнительную услугу (только Admin)
    /// </summary>
    /// <response code="201">Услуга создана</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав (требуется роль Admin)</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ExtraServiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ExtraServiceDto>> Create([FromBody] CreateExtraServiceRequest request)
    {
        var result = await _mediator.Send(new CreateExtraServiceCommand(request));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Обновить дополнительную услугу (только Admin)
    /// </summary>
    /// <param name="id">ID дополнительной услуги</param>
    /// <param name="request">Новые данные услуги</param>
    /// <response code="200">Обновлённые данные услуги</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав (требуется роль Admin)</response>
    /// <response code="404">Услуга не найдена</response>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ExtraServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExtraServiceDto>> Update(int id, [FromBody] UpdateExtraServiceRequest request)
    {
        var result = await _mediator.Send(new UpdateExtraServiceCommand(id, request));
        return Ok(result);
    }

    /// <summary>
    /// Удалить дополнительную услугу (только Admin)
    /// </summary>
    /// <param name="id">ID дополнительной услуги</param>
    /// <response code="204">Услуга удалена</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав (требуется роль Admin)</response>
    /// <response code="404">Услуга не найдена</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteExtraServiceCommand(id));
        return NoContent();
    }
}
