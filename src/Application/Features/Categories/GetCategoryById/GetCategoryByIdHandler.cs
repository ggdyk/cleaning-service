using Application.DTOs.Categories;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Categories.GetCategoryById;

public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
            throw new NotFoundException("Категория", request.Id);

        return new CategoryDto
        {
            Id = category.Id,
            NameRu = category.Name.Ru,
            NameKk = category.Name.Kk,
            NameEn = category.Name.En,
            DescriptionRu = category.Description.Ru,
            DescriptionKk = category.Description.Kk,
            DescriptionEn = category.Description.En,
            IconUrl = category.IconUrl,
            SortOrder = category.SortOrder,
            IsActive = category.IsActive
        };
    }
}
