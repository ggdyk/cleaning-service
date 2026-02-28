using Application.DTOs.Categories;
using MediatR;

namespace Application.Features.Categories.GetCategories;

/// <summary>
/// Запрос списка категорий. onlyActive=true — только активные.
/// </summary>
public record GetCategoriesQuery(bool OnlyActive = true) : IRequest<List<CategoryDto>>;
