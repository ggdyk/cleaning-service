using Application.DTOs.Categories;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Categories.CreateCategory;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        var category = Category.Create(
            req.NameRu,
            req.NameKk,
            req.NameEn,
            req.DescriptionRu,
            req.DescriptionKk,
            req.DescriptionEn,
            req.IconUrl,
            req.SortOrder);

        await _categoryRepository.AddAsync(category, cancellationToken);

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
