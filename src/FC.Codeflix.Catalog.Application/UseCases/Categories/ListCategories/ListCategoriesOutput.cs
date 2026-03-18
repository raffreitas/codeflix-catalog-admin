using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.ListCategories;

public record ListCategoriesOutput(
    int Page,
    int PerPage,
    int Total,
    IReadOnlyList<CategoryModelOutput> Items)
    : PaginatedListOutput<CategoryModelOutput>(Page, PerPage, Total, Items);
