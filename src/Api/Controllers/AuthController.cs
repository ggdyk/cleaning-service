using Application.DTOs.Auth;
using Application.Features.Auth.Login;
using Application.Features.Auth.Logout;
using Application.Features.Auth.RefreshToken;
using Application.Features.Auth.Register;
using Application.Resources;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace Api.Controllers;

/// <summary>
/// Аутентификация и управление сессиями пользователей.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<ErrorMessages> _localizer;

    public AuthController(IMediator mediator, IStringLocalizer<ErrorMessages> localizer)
    {
        _mediator = mediator;
        _localizer = localizer;
    }

    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    /// <remarks>
    /// Пример запроса:
    ///
    ///     POST /api/auth/register
    ///     {
    ///         "email": "ivan@example.com",
    ///         "password": "Password123!",
    ///         "firstName": "Иван",
    ///         "lastName": "Иванов",
    ///         "phone": "+77771234567",
    ///         "city": "Алматы"
    ///     }
    ///
    /// После регистрации используйте `POST /api/auth/login` для получения JWT токена.
    /// </remarks>
    /// <response code="200">Пользователь успешно зарегистрирован</response>
    /// <response code="400">Ошибка валидации (email занят, слабый пароль и т.д.)</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterUserCommand(request);
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Вход пользователя — получение JWT и refresh токенов
    /// </summary>
    /// <remarks>
    /// Пример запроса:
    ///
    ///     POST /api/auth/login
    ///     {
    ///         "email": "ivan@example.com",
    ///         "password": "Password123!"
    ///     }
    ///
    /// Ответ содержит:
    /// - `accessToken` — JWT токен (срок действия 15 минут), передаётся в заголовке `Authorization: Bearer {token}`
    /// - `refreshToken` — для обновления пары токенов через `POST /api/auth/refresh`
    /// - `expiresIn` — срок действия access токена в секундах
    /// </remarks>
    /// <response code="200">Успешный вход, токены в теле ответа</response>
    /// <response code="400">Неверный email или пароль</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request);
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Обновление пары токенов по refresh token
    /// </summary>
    /// <remarks>
    /// Пример запроса:
    ///
    ///     POST /api/auth/refresh
    ///     {
    ///         "refreshToken": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
    ///     }
    ///
    /// Используйте, когда `accessToken` истёк. Выдаётся новая пара токенов, старый refresh token аннулируется.
    /// </remarks>
    /// <response code="200">Новая пара токенов</response>
    /// <response code="400">Refresh token недействителен или истёк</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RefreshTokenResponse>> Refresh([FromBody] RefreshTokenRequest request)
    {
        var command = new RefreshTokenCommand(request);
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Выход пользователя — отзыв всех refresh токенов текущего пользователя
    /// </summary>
    /// <remarks>
    /// Требует авторизации (заголовок `Authorization: Bearer {token}`).
    /// После выхода все refresh токены пользователя аннулируются.
    /// </remarks>
    /// <response code="200">Выход выполнен успешно</response>
    /// <response code="401">Не авторизован</response>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Logout()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException(_localizer["CannotDetermineUser"]);

        var command = new LogoutCommand(userId);
        await _mediator.Send(command);
        return Ok(new { message = _localizer["LogoutSuccess"].Value });
    }
}