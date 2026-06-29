using Codeflix.Catalog.Domain.SeedWork;

namespace Codeflix.Catalog.UnitTests.Application;

public sealed record DomainEventToBeHandledFake : DomainEvent;

public sealed record DomainEventToNotBeHandledFake : DomainEvent;