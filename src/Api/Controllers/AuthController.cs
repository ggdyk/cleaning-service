using Application.DTOs.Auth;
using Application.Features.Auth.Login;
using Application.Features.Auth.Logout;
using Application.Features.Auth.RefreshToken;
using Application.Features.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterUserCommand(request);
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Вход пользователя
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request);
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Обновление access token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResponse>> Refresh([FromBody] RefreshTokenRequest request)
    {
        var command = new RefreshTokenCommand(request);
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Выход пользователя (отзыв refresh токенов)
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Не удалось определить пользователя");

        var command = new LogoutCommand(userId);
        await _mediator.Send(command);
        return Ok(new { message = "Успешный выход" });
    }
}