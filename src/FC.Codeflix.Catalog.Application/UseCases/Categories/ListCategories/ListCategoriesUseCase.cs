using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.ListCategories;
public class ListCategoriesUseCase(ICategoryRepository categoryRepository) : IListCategoriesUseCase
{
    public async Task<ListCategoriesOutput> Handle(ListCategoriesInput request, CancellationToken cancellationToken)
    {
        var searchInput = new SearchInput(
                Page: request.Page,
                PerPage: request.PerPage,
                Search: request.Search,
                OrderBy: request.Sort,
                Order: request.Dir);

        var searchOutput = await categoryRepository.Search(searchInput, cancellationToken);

        return new ListCategoriesOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            [.. searchOutput.Items.Select(CategoryModelOutput.FromCategory)]
        );
    }
}
