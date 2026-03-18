using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Repositories;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.CreateCategory;

public class CreateCategoryUseCase(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : ICreateCategoryUseCase
{
    public async Task<CategoryModelOutput> Handle(CreateCategoryInput input, CancellationToken cancellationToken)
    {
        var category = new Category(input.Name, input.Description, input.IsActive);

        await categoryRepository.Insert(category, cancellationToken);
        await unitOfWork.Commit(cancellationToken);

        return CategoryModelOutput.FromCategory(category);
    }
}