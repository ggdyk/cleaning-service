using Application.DTOs.Content;
using Application.Features.Content.GetAboutPage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Статический контент сайта (страницы «О компании», контакты и т.д.).
/// </summary>
[ApiController]
[Route("api/content")]
[Produces("application/json")]
public class ContentController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Получить контент страницы «О компании»
    /// </summary>
    /// <remarks>Публичный endpoint — авторизация не требуется.</remarks>
    /// <response code="200">Контент страницы</response>
    /// <response code="404">Страница не найдена</response>
    [HttpGet("about")]
    [ProducesResponseType(typeof(PageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PageResponse>> GetAbout(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAboutPageQuery(), ct);
        return Ok(result);
    }
}
