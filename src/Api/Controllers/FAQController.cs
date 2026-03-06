using Api.Authorization;
using Application.DTOs.FAQ;
using Application.Features.FAQ.Admin.CreateFAQ;
using Application.Features.FAQ.Admin.DeleteFAQ;
using Application.Features.FAQ.Admin.GetAllFAQs;
using Application.Features.FAQ.Admin.UpdateFAQ;
using Application.Features.FAQ.GetFAQs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Часто задаваемые вопросы (FAQ). Публичное чтение, управление — Admin/Manager.
/// </summary>
[ApiController]
[Route("api/faq")]
[Produces("application/json")]
public class FAQController : ControllerBase
{
    private readonly IMediator _mediator;

    public FAQController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // =========================================================================
    // Публичные endpoints
    // =========================================================================

    /// <summary>
    /// Получить список активных FAQ
    /// </summary>
    /// <remarks>
    /// Публичный endpoint — авторизация не требуется.
    /// Язык ответа определяется по заголовку `Accept-Language` или параметру `?lang=ru|kk|en` (по умолчанию ru).
    /// Возвращает только активные (IsActive = true) записи, отсортированные по полю Order.
    /// </remarks>
    /// <response code="200">Список FAQ на запрошенном языке</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<FaqLocalizedResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<FaqLocalizedResponse>>> GetActive(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetFAQsQuery(), ct);
        return Ok(result);
    }

    // =========================================================================
    // Админские endpoints (только Admin и Manager)
    // =========================================================================

    /// <summary>
    /// Получить все FAQ включая неактивные (Admin/Manager)
    /// </summary>
    /// <response code="200">Полный список FAQ со статусами</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав (требуется роль Admin или Manager)</response>
    [HttpGet("admin")]
    [Authorize(Policy = Policies.AdminOrManager)]
    [ProducesResponseType(typeof(IReadOnlyList<FaqResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<FaqResponse>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllFAQsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Создать новый FAQ (Admin/Manager)
    /// </summary>
    /// <response code="201">FAQ создан</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав (требуется роль Admin или Manager)</response>
    [HttpPost("admin")]
    [Authorize(Policy = Policies.AdminOrManager)]
    [ProducesResponseType(typeof(FaqResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<FaqResponse>> Create([FromBody] CreateFaqRequest request)
    {
        var result = await _mediator.Send(new CreateFAQCommand(request));
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    /// <summary>
    /// Обновить FAQ (Admin/Manager)
    /// </summary>
    /// <param name="id">ID записи FAQ</param>
    /// <param name="request">Новые данные FAQ</param>
    /// <response code="200">Обновлённые данные FAQ</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав (требуется роль Admin или Manager)</response>
    /// <response code="404">FAQ не найден</response>
    [HttpPut("admin/{id:int}")]
    [Authorize(Policy = Policies.AdminOrManager)]
    [ProducesResponseType(typeof(FaqResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FaqResponse>> Update(int id, [FromBody] UpdateFaqRequest request)
    {
        var result = await _mediator.Send(new UpdateFAQCommand(id, request));
        return Ok(result);
    }

    /// <summary>
    /// Удалить FAQ (Admin/Manager)
    /// </summary>
    /// <param name="id">ID записи FAQ</param>
    /// <response code="204">FAQ удалён</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав (требуется роль Admin или Manager)</response>
    /// <response code="404">FAQ не найден</response>
    [HttpDelete("admin/{id:int}")]
    [Authorize(Policy = Policies.AdminOrManager)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteFAQCommand(id));
        return NoContent();
    }
}
