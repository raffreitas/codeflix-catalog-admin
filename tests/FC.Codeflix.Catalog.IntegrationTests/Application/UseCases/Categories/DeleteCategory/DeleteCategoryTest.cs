using FC.Codeflix.Catalog.Application;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Categories.DeleteCategory;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Categories.DeleteCategory;

[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTest(DeleteCategoryTestFixture fixture)
{
    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("Integration/Application", "DeleteCategory - Use Cases")]
    public async Task DeleteCategory()
    {
        var categoryExample = fixture.GetExampleCategory();
        var categoryExampleList = fixture.GetExampleCategoryList();
        var dbContext = fixture.CreateDbContext();
        await dbContext.Categories.AddRangeAsync(categoryExampleList);
        var trackingInfo = await dbContext.Categories.AddAsync(categoryExample);
        await dbContext.SaveChangesAsync();
        trackingInfo.State = EntityState.Detached;
        var repository = new CategoryRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            dbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );

        var input = new DeleteCategoryInput(categoryExample.Id);
        var useCase = new DeleteCategoryUseCase(repository, unitOfWork);

        await useCase.Handle(input, CancellationToken.None);

        var dbCategory = await fixture
            .CreateDbContext(preserveData: true)
            .Categories
            .SingleOrDefaultAsync(x => x.Id == categoryExample.Id);
        dbCategory.Should().BeNull();

        var dbCount = await fixture
            .CreateDbContext(preserveData: true)
            .Categories
            .CountAsync();
        dbCount.Should().Be(categoryExampleList.Count);
    }

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Integration/Application", "DeleteCategory - Use Cases")]
    public async Task ThrowWhenCategoryNotFound()
    {
        var exampleGuid = Guid.Empty;
        var dbContext = fixture.CreateDbContext();
        await dbContext.Categories.AddAsync(fixture.GetExampleCategory());
        await dbContext.SaveChangesAsync();
        var repository = new CategoryRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            dbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );

        var input = new DeleteCategoryInput(exampleGuid);
        var useCase = new DeleteCategoryUseCase(repository, unitOfWork);

        var act = async () => await useCase.Handle(input, CancellationToken.None);

        await act.Should()
            .ThrowAsync<NotFoundException>();

        var dbCount = await fixture
            .CreateDbContext(preserveData: true)
            .Categories
            .CountAsync();

        dbCount.Should().Be(1);
    }
}