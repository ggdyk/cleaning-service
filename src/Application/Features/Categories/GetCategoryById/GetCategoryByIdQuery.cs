using Application.DTOs.Categories;
using MediatR;

namespace Application.Features.Categories.GetCategoryById;

/// <summary>
/// Запрос конкретной категории по Id.
/// </summary>
public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto>;
