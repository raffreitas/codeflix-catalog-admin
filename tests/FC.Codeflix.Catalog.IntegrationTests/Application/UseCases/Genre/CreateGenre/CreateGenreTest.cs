using FC.Codeflix.Catalog.Application;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

using FluentAssertions;

using FC.Codeflix.Catalog.Infra.Data.EF.Models;

using Microsoft.EntityFrameworkCore;

using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genres.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genres.CreateGenre;
using FC.Codeflix.Catalog.Domain.Entities;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest
{
    private readonly CreateGenreTestFixture _fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    public async Task CreateGenre()
    {
        CreateGenreInput input = _fixture.GetExampleInput();
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            actDbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        CreateGenreUseCase createGenre = new CreateGenreUseCase(
            new GenreRepository(actDbContext),
            new CategoryRepository(actDbContext),
            unitOfWork
        );

        GenreModelOutput output = await createGenre.Handle(
            input,
            CancellationToken.None
        );

        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Categories.Should().HaveCount(0);
        CodeflixCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
        Domain.Entities.Genre? genreFromDb =
            await assertDbContext.Genres.FindAsync(output.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be(input.IsActive);
    }

    [Fact(DisplayName = nameof(CreateGenreWithCategoriesRelations))]
    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    public async Task CreateGenreWithCategoriesRelations()
    {
        List<Category> exampleCategories = _fixture.GetExampleCategoriesList(5);
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.SaveChangesAsync();
        CreateGenreInput input = _fixture.GetExampleInput();
        input.CategoriesIds = exampleCategories
            .Select(category => category.Id).ToList();
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            actDbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        CreateGenreUseCase createGenre = new CreateGenreUseCase(
            new GenreRepository(actDbContext),
            new CategoryRepository(actDbContext),
            unitOfWork
        );

        GenreModelOutput output = await createGenre.Handle(
            input,
            CancellationToken.None
        );

        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Categories.Should().HaveCount(input.CategoriesIds.Count);
        List<Guid> relatedCategoriesIdsFromOutput = output.Categories
            .Select(relation => relation.Id).ToList();
        relatedCategoriesIdsFromOutput.Should().BeEquivalentTo(input.CategoriesIds);
        CodeflixCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
        Domain.Entities.Genre? genreFromDb =
            await assertDbContext.Genres.FindAsync(output.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be(input.IsActive);
        List<GenresCategories> relations =
            await assertDbContext.GenresCategories.AsNoTracking()
                .Where(x => x.GenreId == output.Id)
                .ToListAsync();
        relations.Should().HaveCount(input.CategoriesIds.Count);
        List<Guid> categoryIdsRelatedFromDb =
            relations.Select(relation => relation.CategoryId).ToList();
        categoryIdsRelatedFromDb.Should().BeEquivalentTo(input.CategoriesIds);
    }

    [Fact(DisplayName = nameof(CreateGenreThrowsWhenCategoryDoesntExists))]
    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    public async Task CreateGenreThrowsWhenCategoryDoesntExists()
    {
        List<Category> exampleCategories = _fixture.GetExampleCategoriesList(5);
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.SaveChangesAsync();
        CreateGenreInput input = _fixture.GetExampleInput();
        input.CategoriesIds = exampleCategories
            .Select(category => category.Id).ToList();
        Guid randomGuid = Guid.NewGuid();
        input.CategoriesIds.Add(randomGuid);
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            actDbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        CreateGenreUseCase createGenre = new CreateGenreUseCase(
            new GenreRepository(actDbContext),
            new CategoryRepository(actDbContext),
            unitOfWork
        );

        Func<Task<GenreModelOutput>> action = async () => await createGenre.Handle(
            input,
            CancellationToken.None
        );

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {randomGuid}");
    }
}