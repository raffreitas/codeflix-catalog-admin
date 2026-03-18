using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;
using FC.Codeflix.Catalog.Domain.Repositories;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.GetCategory;

public class GetCategoryUseCase(ICategoryRepository categoryRepository) : IGetCategoryUseCase
{
    public async Task<CategoryModelOutput> Handle(GetCategoryInput request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.Get(request.Id, cancellationToken);

        return CategoryModelOutput.FromCategory(category);
    }
}