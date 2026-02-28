using Application.DTOs.Categories;
using MediatR;

namespace Application.Features.Categories.UpdateCategory;

/// <summary>
/// Команда обновления категории (только Admin).
/// </summary>
public record UpdateCategoryCommand(int Id, UpdateCategoryRequest Request) : IRequest<CategoryDto>;
