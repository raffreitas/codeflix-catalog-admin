using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.ListCategories;
public class ListCategoriesUseCase : IListCategoriesUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public ListCategoriesUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ListCategoriesOutput> Handle(ListCategoriesInput request, CancellationToken cancellationToken)
    {
        var searchInput = new SearchInput(
                page: request.Page,
                perPage: request.PerPage,
                search: request.Search,
                orderBy: request.Sort,
                order: request.Dir);

        var searchOutput = await _categoryRepository.Search(searchInput, cancellationToken);

        return new ListCategoriesOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            [.. searchOutput.Items.Select(CategoryModelOutput.FromCategory)]
        );
    }
}
