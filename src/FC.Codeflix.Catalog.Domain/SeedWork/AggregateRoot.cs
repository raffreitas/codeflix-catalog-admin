using System.Collections.ObjectModel;

namespace FC.Codeflix.Catalog.Domain.SeedWork;

public abstract class AggregateRoot : Entity
{
    private readonly List<DomainEvent> _domainEvents = [];
    public IReadOnlyCollection<DomainEvent> DomainEvents => new ReadOnlyCollection<DomainEvent>(_domainEvents);
    public void RaiseEvent(DomainEvent @event) => _domainEvents.Add(@event);
    public void ClearEvents() => _domainEvents.Clear();
}