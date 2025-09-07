using FC.Codeflix.Catalog.Domain.SeedWork;

namespace FC.Codeflix.Catalog.UnitTests.Application;

public sealed record DomainEventToBeHandledFake : DomainEvent;

public sealed record DomainEventToNotBeHandledFake : DomainEvent;