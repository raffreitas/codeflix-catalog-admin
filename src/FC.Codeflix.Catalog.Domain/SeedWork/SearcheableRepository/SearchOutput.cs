namespace FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

public record SearchOutput<TAggregate>(
    int CurrentPage,
    int PerPage,
    int Total,
    IReadOnlyList<TAggregate> Items)
    where TAggregate : AggregateRoot
{
    public int CurrentPage { get; set; } = CurrentPage;
    public int PerPage { get; set; } = PerPage;
    public int Total { get; set; } = Total;
    public IReadOnlyList<TAggregate> Items { get; set; } = Items;
}
