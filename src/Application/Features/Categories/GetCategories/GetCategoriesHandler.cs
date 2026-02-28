using Application.DTOs.Categories;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Categories.GetCategories;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery query, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync(query.OnlyActive, cancellationToken);

        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            NameRu = c.Name.Ru,
            NameKk = c.Name.Kk,
            NameEn = c.Name.En,
            DescriptionRu = c.Description.Ru,
            DescriptionKk = c.Description.Kk,
            DescriptionEn = c.Description.En,
            IconUrl = c.IconUrl,
            SortOrder = c.SortOrder,
            IsActive = c.IsActive
        }).ToList();
    }
}
