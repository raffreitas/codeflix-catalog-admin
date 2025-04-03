using FC.Codeflix.Catalog.Domain.Repositories;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.GetCategory;

public class GetCategoryUseCase : IGetCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;
    public GetCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<GetCategoryOutput> Handle(GetCategoryInput request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.Get(request.Id, cancellationToken);

        return GetCategoryOutput.FromCategory(category);
    }
}
