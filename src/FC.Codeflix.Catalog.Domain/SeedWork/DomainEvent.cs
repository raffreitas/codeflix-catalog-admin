namespace FC.Codeflix.Catalog.Domain.SeedWork;

public abstract record DomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
};