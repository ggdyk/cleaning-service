using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Categories.DeleteCategory;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
            throw new NotFoundException("Категория", request.Id);

        await _categoryRepository.DeleteAsync(category, cancellationToken);
    }
}
