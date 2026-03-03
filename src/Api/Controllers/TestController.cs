using Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

/// <summary>
/// Тестовые endpoints для проверки работы политик авторизации.
/// Используются в разработке — убрать перед деплоем в Production.
/// </summary>
[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    /// <summary>Публичный endpoint — без авторизации.</summary>
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult Public()
        => Ok(new { message = "Публичный endpoint — доступен всем." });

    /// <summary>
    /// Требует любую валидную авторизацию.
    /// Возвращает userId, email и роль из JWT claims.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;
        var email  = User.FindFirst(ClaimTypes.Email)?.Value
                     ?? User.FindFirst("email")?.Value;
        var role   = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new { userId, email, role });
    }

    /// <summary>Только Admin. Возвращает 403 для всех остальных ролей.</summary>
    [HttpGet("admin-only")]
    [Authorize(Policy = Policies.AdminOnly)]
    public IActionResult AdminOnly()
        => Ok(new { message = "Доступ разрешён: Admin." });

    /// <summary>Admin или Manager.</summary>
    [HttpGet("admin-or-manager")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public IActionResult AdminOrManager()
        => Ok(new { message = "Доступ разрешён: Admin или Manager." });

    /// <summary>Только Cleaner.</summary>
    [HttpGet("cleaner-only")]
    [Authorize(Policy = Policies.CleanerOnly)]
    public IActionResult CleanerOnly()
        => Ok(new { message = "Доступ разрешён: Cleaner." });

    /// <summary>Любой сотрудник: Admin, Manager, Cleaner.</summary>
    [HttpGet("staff")]
    [Authorize(Policy = Policies.Staff)]
    public IActionResult Staff()
        => Ok(new { message = "Доступ разрешён: Staff (Admin/Manager/Cleaner)." });
}
