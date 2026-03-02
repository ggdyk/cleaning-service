using Application.DTOs.Content;
using Application.Features.Content.GetAboutPage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/content")]
public class ContentController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Получить контент страницы «О компании». Публичный доступ.
    /// </summary>
    [HttpGet("about")]
    public async Task<ActionResult<PageResponse>> GetAbout(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAboutPageQuery(), ct);
        return Ok(result);
    }
}
