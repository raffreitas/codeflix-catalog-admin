using Codeflix.Catalog.Application.Common;
using Codeflix.Catalog.Application.UseCases.Categories.Common;

namespace Codeflix.Catalog.Application.UseCases.Categories.ListCategories;

public record ListCategoriesOutput(
    int Page,
    int PerPage,
    int Total,
    IReadOnlyList<CategoryModelOutput> Items)
    : PaginatedListOutput<CategoryModelOutput>(Page, PerPage, Total, Items);
