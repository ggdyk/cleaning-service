using MediatR;

namespace Application.Features.Categories.DeleteCategory;

/// <summary>
/// Команда удаления категории (только Admin).
/// </summary>
public record DeleteCategoryCommand(int Id) : IRequest;
