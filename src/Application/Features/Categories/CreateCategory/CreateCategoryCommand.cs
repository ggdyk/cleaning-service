using Application.DTOs.Categories;
using MediatR;

namespace Application.Features.Categories.CreateCategory;

/// <summary>
/// Команда создания категории (только Admin).
/// </summary>
public record CreateCategoryCommand(CreateCategoryRequest Request) : IRequest<CategoryDto>;
