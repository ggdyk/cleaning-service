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

[ApiController]
[Route("api/[controller]")]
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
    [HttpGet]
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
    [HttpGet("{id}")]
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
    [HttpPost]
    [Authorize(Roles = "Admin")]
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
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
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
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(
        [FromRoute] int id)
    {
        var command = new DeleteCategoryCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}
