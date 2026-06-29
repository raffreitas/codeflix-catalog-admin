namespace Codeflix.Catalog.Application.Common;

public abstract record PaginatedListOutput<TOutputItem>(
    int Page,
    int PerPage,
    int Total,
    IReadOnlyList<TOutputItem> Items)
{
    public int Page { get; set; } = Page;
    public int PerPage { get; set; } = PerPage;
    public int Total { get; set; } = Total;
    public IReadOnlyList<TOutputItem> Items { get; set; } = Items;
}
