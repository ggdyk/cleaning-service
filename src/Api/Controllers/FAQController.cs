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

[ApiController]
[Route("api/faq")]
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
    /// Получить список активных FAQ на языке запроса.
    /// Язык: ?lang=ru|kk|en или заголовок Accept-Language. По умолчанию ru.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<FaqLocalizedResponse>>> GetActive(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetFAQsQuery(), ct);
        return Ok(result);
    }

    // =========================================================================
    // Админские endpoints (только Admin и Manager)
    // =========================================================================

    /// <summary>
    /// Получить все FAQ включая неактивные. Только для Admin/Manager.
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IReadOnlyList<FaqResponse>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllFAQsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Создать новый FAQ. Только для Admin/Manager.
    /// </summary>
    [HttpPost("admin")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<FaqResponse>> Create([FromBody] CreateFaqRequest request)
    {
        var result = await _mediator.Send(new CreateFAQCommand(request));
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    /// <summary>
    /// Обновить FAQ по id. Только для Admin/Manager.
    /// </summary>
    [HttpPut("admin/{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<FaqResponse>> Update(int id, [FromBody] UpdateFaqRequest request)
    {
        var result = await _mediator.Send(new UpdateFAQCommand(id, request));
        return Ok(result);
    }

    /// <summary>
    /// Удалить FAQ по id. Только для Admin/Manager.
    /// </summary>
    [HttpDelete("admin/{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteFAQCommand(id));
        return NoContent();
    }
}
