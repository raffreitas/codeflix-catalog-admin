using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.ListCategories;

public record ListCategoriesOutput : PaginatedListOutput<CategoryModelOutput>
{
    public ListCategoriesOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<CategoryModelOutput> items) : base(page, perPage, total, items)
    { }
}
