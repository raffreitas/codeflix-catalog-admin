using Codeflix.Catalog.Application.Common;

using Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Categories.ListCategories;
public record ListCategoriesInput(
    int Page = 1,
    int PerPage = 15,
    string Search = "",
    string Sort = "",
    SearchOrder Dir = SearchOrder.Asc)
    : PaginatedListInput(Page, PerPage, Search, Sort, Dir), IRequest<ListCategoriesOutput>
{
    public ListCategoriesInput()
        : this(1, 15, "", "", SearchOrder.Asc)
    {
    }
}
