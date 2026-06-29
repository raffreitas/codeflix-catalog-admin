using Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

namespace Codeflix.Catalog.Application.Common;
public abstract record PaginatedListInput(
    int Page,
    int PerPage,
    string Search,
    string Sort,
    SearchOrder Dir)
{
    public int Page { get; set; } = Page;
    public int PerPage { get; set; } = PerPage;
    public string Search { get; set; } = Search;
    public string Sort { get; set; } = Sort;
    public SearchOrder Dir { get; set; } = Dir;

    public SearchInput ToSearchInput()
        => new(
            Page: Page,
            PerPage: PerPage,
            Search: Search,
            OrderBy: Sort,
            Order: Dir
        );
}
