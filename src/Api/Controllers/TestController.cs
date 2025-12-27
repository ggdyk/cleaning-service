using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    /// <summary>
    /// Публичный endpoint (без авторизации)
    /// </summary>
    [HttpGet("public")]
    public IActionResult Public()
    {
        return Ok(new { message = "Это публичный endpoint, доступен всем" });
    }

    /// <summary>
    /// Защищённый endpoint (требует авторизацию)
    /// </summary>
    [Authorize]
    [HttpGet("protected")]
    public IActionResult Protected()
    {
        var userId = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;
        var role = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

        return Ok(new
        {
            message = "Вы авторизованы!",
            userId = userId,
            email = email,
            role = role
        });
    }

    /// <summary>
    /// Endpoint только для админов
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("admin-only")]
    public IActionResult AdminOnly()
    {
        return Ok(new { message = "Доступ разрешён только администраторам" });
    }

    /// <summary>
    /// Endpoint для клиентов и админов
    /// </summary>
    [Authorize(Roles = "Client,Admin")]
    [HttpGet("client-or-admin")]
    public IActionResult ClientOrAdmin()
    {
        var role = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
        return Ok(new { message = $"Доступ разрешён для роли: {role}" });
    }
}