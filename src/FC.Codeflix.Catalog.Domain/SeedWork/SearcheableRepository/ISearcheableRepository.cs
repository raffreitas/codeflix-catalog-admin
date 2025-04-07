namespace FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;
public interface ISearchableRepository<TAggregate> where TAggregate : AggregateRoot
{
    public Task<SearchOutput<TAggregate>> Search(
        SearchInput input,
        CancellationToken cancellationToken = default
    );
}
