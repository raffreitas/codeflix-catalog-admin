using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.SeedWork;

[Trait("Domain", "AggregateRoot - SeedWork")]
public sealed class AggregateRootTest
{
    [Fact(DisplayName = nameof(RaiseEvent))]
    public void RaiseEvent()
    {
        var domainEvent = new DomainEventFake();
        var aggregate = new AggregateRootFake();

        aggregate.RaiseEvent(domainEvent);

        aggregate.DomainEvents.Should().HaveCount(1);
    }

    [Fact(DisplayName = nameof(ClearEvents))]
    public void ClearEvents()
    {
        var domainEvent = new DomainEventFake();
        var aggregate = new AggregateRootFake();
        aggregate.RaiseEvent(domainEvent);

        aggregate.ClearEvents();

        aggregate.DomainEvents.Should().BeEmpty();
    }
}