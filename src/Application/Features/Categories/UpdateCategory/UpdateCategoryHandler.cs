using Application.DTOs.Categories;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Categories.UpdateCategory;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
            throw new NotFoundException("Категория", request.Id);

        var req = request.Request;

        category.Update(
            req.NameRu,
            req.NameKk,
            req.NameEn,
            req.DescriptionRu,
            req.DescriptionKk,
            req.DescriptionEn,
            req.IconUrl,
            req.SortOrder,
            req.IsActive);

        await _categoryRepository.UpdateAsync(category, cancellationToken);

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
