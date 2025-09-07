using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.SeedWork;

using Microsoft.Extensions.Logging;

namespace FC.Codeflix.Catalog.Infra.Data.EF;

public sealed class UnitOfWork(
    CodeflixCatalogDbContext dbContext,
    IDomainEventPublisher publisher,
    ILogger<UnitOfWork> logger
) : IUnitOfWork
{
    public async Task Commit(CancellationToken cancellationToken = default)
    {
        var aggregateRoots = dbContext.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(entry => entry.Entity.DomainEvents.Count != 0)
            .Select(entry => entry.Entity)
            .ToList();

        logger.LogInformation("Commit: {AggregatesCount} aggregate roots with events.", aggregateRoots.Count);

        var events = aggregateRoots
            .SelectMany(aggregate => aggregate.DomainEvents)
            .ToList();

        logger.LogInformation("Commit: {DomainEventsCount} events raised.", events.Count);

        foreach (var @event in events)
            await publisher.PublishAsync((dynamic)@event, cancellationToken);

        foreach (var aggregate in aggregateRoots)
            aggregate.ClearEvents();

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task Rollback(CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}