using Application.DTOs.Categories;
using Application.Features.Categories.CreateCategory;
using Application.Features.Categories.DeleteCategory;
using Application.Features.Categories.GetCategories;
using Application.Features.Categories.GetCategoryById;
using Application.Features.Categories.UpdateCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Управление категориями услуг. Чтение — публичное. Запись — только Admin.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Получить список категорий
    /// </summary>
    /// <param name="onlyActive">Если true — только активные (по умолчанию true)</param>
    /// <response code="200">Список категорий</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoryDto>>> GetAll(
        [FromQuery] bool onlyActive = true)
    {
        var query = new GetCategoriesQuery(onlyActive);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Получить категорию по ID
    /// </summary>
    /// <param name="id">ID категории</param>
    /// <response code="200">Данные категории</response>
    /// <response code="404">Категория не найдена</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetById(
        [FromRoute] int id)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Создать категорию (только Admin)
    /// </summary>
    /// <response code="201">Категория создана</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав (требуется роль Admin)</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CategoryDto>> Create(
        [FromBody] CreateCategoryRequest request)
    {
        var command = new CreateCategoryCommand(request);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Обновить категорию (только Admin)
    /// </summary>
    /// <param name="id">ID категории</param>
    /// <param name="request">Новые данные категории</param>
    /// <response code="200">Обновлённые данные категории</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав (требуется роль Admin)</response>
    /// <response code="404">Категория не найдена</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> Update(
        [FromRoute] int id,
        [FromBody] UpdateCategoryRequest request)
    {
        var command = new UpdateCategoryCommand(id, request);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Удалить категорию (только Admin)
    /// </summary>
    /// <param name="id">ID категории</param>
    /// <response code="204">Категория удалена</response>
    /// <response code="401">Не авторизован</response>
    /// <response code="403">Недостаточно прав (требуется роль Admin)</response>
    /// <response code="404">Категория не найдена</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(
        [FromRoute] int id)
    {
        var command = new DeleteCategoryCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}
