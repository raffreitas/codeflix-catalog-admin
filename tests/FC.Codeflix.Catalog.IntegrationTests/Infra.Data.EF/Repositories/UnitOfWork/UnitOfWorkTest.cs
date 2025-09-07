using FC.Codeflix.Catalog.Application;
using FC.Codeflix.Catalog.Domain.SeedWork;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using UoW = FC.Codeflix.Catalog.Infra.Data.EF;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.UnitOfWork;

[Collection(nameof(UnitOfWorkTestFixture))]
public class UnitOfWorkTest(UnitOfWorkTestFixture fixture)
{
    [Fact(DisplayName = nameof(Commit))]
    [Trait("Integration/Infra.Data.EF", "UnitOfWork - Persistence")]
    public async Task Commit()
    {
        var dbContext = fixture.CreateDbContext();
        var categoriesList = fixture.GetExampleCategoryList();
        var categoryWithEvent = categoriesList.First();
        var @event = new DomainEventFake();
        categoryWithEvent.RaiseEvent(@event);
        var eventHandlerMock = new Mock<IDomainEventHandler<DomainEventFake>>();
        await dbContext.AddRangeAsync(categoriesList);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        serviceCollection.AddSingleton(eventHandlerMock.Object);
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UoW.UnitOfWork(
            dbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UoW.UnitOfWork>>()
        );

        await unitOfWork.Commit();

        var categories = await fixture
            .CreateDbContext(preserveData: true)
            .Categories
            .AsNoTracking()
            .ToListAsync();

        categories.Should().HaveCount(categoriesList.Count);
        eventHandlerMock.Verify(x => x
            .HandleAsync(@event, It.IsAny<CancellationToken>()), Times.Once);
        categoryWithEvent.DomainEvents.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(Rollback))]
    [Trait("Integration/Infra.Data.EF", "UnitOfWork - Persistence")]
    public async Task Rollback()
    {
        var dbContext = fixture.CreateDbContext();
        var categoriesList = fixture.GetExampleCategoryList();
        await dbContext.AddRangeAsync(categoriesList);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UoW.UnitOfWork(
            dbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UoW.UnitOfWork>>()
        );

        await unitOfWork.Rollback();

        var categories = await fixture
            .CreateDbContext(preserveData: true)
            .Categories
            .AsNoTracking()
            .ToListAsync();

        categories.Should().BeEmpty();
    }
}