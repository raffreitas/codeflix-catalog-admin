﻿namespace FC.Codeflix.Catalog.Domain.SeedWork;

public interface IDomainEventPublisher
{
    Task PublishAsync<TDomainEvent>(TDomainEvent @event, CancellationToken cancellationToken = default)
        where TDomainEvent : DomainEvent;
}