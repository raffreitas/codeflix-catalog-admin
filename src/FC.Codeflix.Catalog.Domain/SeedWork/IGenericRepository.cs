namespace FC.Codeflix.Catalog.Domain.SeedWork;

public interface IGenericRepository<TAggregate> : IRepository
    where TAggregate : AggregateRoot
{
    public Task Insert(TAggregate aggregate, CancellationToken cancellationToken = default);
    public Task<TAggregate> Get(Guid id, CancellationToken cancellationToken = default);
}