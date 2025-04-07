using FC.Codeflix.Catalog.Domain.SeedWork;

namespace FC.Codeflix.Catalog.Application.Common;

public abstract record PaginatedListOutput<TAggregate>
    where TAggregate : AggregateRoot
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public int Total { get; set; }
    public IReadOnlyList<TAggregate> Items { get; set; }
    public PaginatedListOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<TAggregate> items)
    {
        Page = page;
        PerPage = perPage;
        Total = total;
        Items = items;
    }
}
