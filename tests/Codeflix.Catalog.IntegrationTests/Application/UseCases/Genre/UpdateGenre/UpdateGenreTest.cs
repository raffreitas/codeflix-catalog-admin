using Codeflix.Catalog.Application;
using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Application.UseCases.Genres.Common;
using Codeflix.Catalog.Application.UseCases.Genres.UpdateGenre;
using Codeflix.Catalog.Infra.Data.EF;
using Codeflix.Catalog.Infra.Data.EF.Models;
using Codeflix.Catalog.Infra.Data.EF.Repositories;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using DomainEntity = Codeflix.Catalog.Domain.Entities;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.UpdateGenre;

[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTest(UpdateGenreTestFixture fixture)
{
    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenre()
    {
        List<DomainEntity.Genre> exampleGenres = fixture.GetExampleListGenres();
        CodeflixCatalogDbContext arrangeDbContext = fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContext = fixture.CreateDbContext(true);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            actDbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        UpdateGenreUseCase updateGenre = new UpdateGenreUseCase(
            new GenreRepository(actDbContext),
            unitOfWork,
            new CategoryRepository(actDbContext)
        );
        UpdateGenreInput input = new UpdateGenreInput(
            targetGenre.Id,
            fixture.GetValidGenreName(),
            !targetGenre.IsActive
        );

        GenreModelOutput output = await updateGenre.Handle(
            input,
            CancellationToken.None
        );

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        CodeflixCatalogDbContext assertDbContext = fixture.CreateDbContext(true);
        DomainEntity.Genre? genreFromDb =
            await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
    }

    [Fact(DisplayName = nameof(UpdateGenreWithCategoriesRelations))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithCategoriesRelations()
    {
        List<DomainEntity.Category> exampleCategories = fixture.GetExampleCategoriesList();
        List<DomainEntity.Genre> exampleGenres = fixture.GetExampleListGenres();
        CodeflixCatalogDbContext arrangeDbContext = fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
        List<DomainEntity.Category> newRelatedCategories = exampleCategories.GetRange(5, 3);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        List<GenresCategories> relations = targetGenre.Categories
            .Select(categoryId => new GenresCategories(targetGenre.Id, categoryId))
            .ToList();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(relations);
        await arrangeDbContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContext = fixture.CreateDbContext(true);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            actDbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        UpdateGenreUseCase updateGenre = new UpdateGenreUseCase(
            new GenreRepository(actDbContext),
            unitOfWork,
            new CategoryRepository(actDbContext)
        );
        UpdateGenreInput input = new UpdateGenreInput(
            targetGenre.Id,
            fixture.GetValidGenreName(),
            !targetGenre.IsActive,
            newRelatedCategories.Select(category => category.Id).ToList()
        );

        GenreModelOutput output = await updateGenre.Handle(
            input,
            CancellationToken.None
        );

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.Categories.Should().HaveCount(newRelatedCategories.Count);
        List<Guid> relatedCategoryIdsFromOutput =
            output.Categories.Select(relatedCategory => relatedCategory.Id).ToList();
        relatedCategoryIdsFromOutput.Should().BeEquivalentTo(input.CategoriesIds);
        CodeflixCatalogDbContext assertDbContext = fixture.CreateDbContext(true);
        DomainEntity.Genre? genreFromDb =
            await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
        List<Guid> relatedcategoryIdsFromDb = await assertDbContext
            .GenresCategories.AsNoTracking()
            .Where(relation => relation.GenreId == input.Id)
            .Select(relation => relation.CategoryId)
            .ToListAsync();
        relatedcategoryIdsFromDb.Should().BeEquivalentTo(input.CategoriesIds);
    }

    [Fact(DisplayName = nameof(UpdateGenreWithoutNewCategoriesRelations))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithoutNewCategoriesRelations()
    {
        List<DomainEntity.Category> exampleCategories = fixture.GetExampleCategoriesList();
        List<DomainEntity.Genre> exampleGenres = fixture.GetExampleListGenres();
        CodeflixCatalogDbContext arrangeDbContext = fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        List<GenresCategories> relations = targetGenre.Categories
            .Select(categoryId => new GenresCategories(targetGenre.Id, categoryId))
            .ToList();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(relations);
        await arrangeDbContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContext = fixture.CreateDbContext(true);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            actDbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        UpdateGenreUseCase updateGenre = new UpdateGenreUseCase(
            new GenreRepository(actDbContext),
            unitOfWork,
            new CategoryRepository(actDbContext)
        );
        UpdateGenreInput input = new UpdateGenreInput(
            targetGenre.Id,
            fixture.GetValidGenreName(),
            !targetGenre.IsActive
        );

        GenreModelOutput output = await updateGenre.Handle(
            input,
            CancellationToken.None
        );

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.Categories.Should().HaveCount(relatedCategories.Count);
        List<Guid> expectedRelatedCategoryIds = relatedCategories
            .Select(category => category.Id).ToList();
        List<Guid> relatedCategoryIdsFromOutput =
            output.Categories.Select(relatedCategory => relatedCategory.Id).ToList();
        relatedCategoryIdsFromOutput.Should().BeEquivalentTo(expectedRelatedCategoryIds);
        CodeflixCatalogDbContext assertDbContext = fixture.CreateDbContext(true);
        DomainEntity.Genre? genreFromDb =
            await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
        List<Guid> relatedcategoryIdsFromDb = await assertDbContext
            .GenresCategories.AsNoTracking()
            .Where(relation => relation.GenreId == input.Id)
            .Select(relation => relation.CategoryId)
            .ToListAsync();
        relatedcategoryIdsFromDb.Should().BeEquivalentTo(expectedRelatedCategoryIds);
    }

    [Fact(DisplayName = nameof(UpdateGenreWithEmptyCategoryIdsCleanRelations))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithEmptyCategoryIdsCleanRelations()
    {
        List<DomainEntity.Category> exampleCategories = fixture.GetExampleCategoriesList();
        List<DomainEntity.Genre> exampleGenres = fixture.GetExampleListGenres();
        CodeflixCatalogDbContext arrangeDbContext = fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        List<GenresCategories> relations = targetGenre.Categories
            .Select(categoryId => new GenresCategories(targetGenre.Id, categoryId))
            .ToList();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(relations);
        await arrangeDbContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContext = fixture.CreateDbContext(true);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            actDbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        UpdateGenreUseCase updateGenre = new UpdateGenreUseCase(
            new GenreRepository(actDbContext),
            unitOfWork,
            new CategoryRepository(actDbContext)
        );
        UpdateGenreInput input = new UpdateGenreInput(
            targetGenre.Id,
            fixture.GetValidGenreName(),
            !targetGenre.IsActive,
            new List<Guid>()
        );

        GenreModelOutput output = await updateGenre.Handle(
            input,
            CancellationToken.None
        );

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.Categories.Should().HaveCount(0);
        List<Guid> relatedCategoryIdsFromOutput =
            output.Categories.Select(relatedCategory => relatedCategory.Id).ToList();
        relatedCategoryIdsFromOutput.Should().BeEquivalentTo(new List<Guid>());
        CodeflixCatalogDbContext assertDbContext = fixture.CreateDbContext(true);
        DomainEntity.Genre? genreFromDb =
            await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
        List<Guid> relatedcategoryIdsFromDb = await assertDbContext
            .GenresCategories.AsNoTracking()
            .Where(relation => relation.GenreId == input.Id)
            .Select(relation => relation.CategoryId)
            .ToListAsync();
        relatedcategoryIdsFromDb.Should().BeEquivalentTo(new List<Guid>());
    }

    [Fact(DisplayName = nameof(UpdateGenreThrowsWhenCategoryDoesntExists))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreThrowsWhenCategoryDoesntExists()
    {
        List<DomainEntity.Category> exampleCategories = fixture.GetExampleCategoriesList();
        List<DomainEntity.Genre> exampleGenres = fixture.GetExampleListGenres();
        CodeflixCatalogDbContext arrangeDbContext = fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
        List<DomainEntity.Category> newRelatedCategories = exampleCategories.GetRange(5, 3);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        List<GenresCategories> relations = targetGenre.Categories
            .Select(categoryId => new GenresCategories(targetGenre.Id, categoryId))
            .ToList();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(relations);
        await arrangeDbContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContext = fixture.CreateDbContext(true);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            actDbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        UpdateGenreUseCase updateGenre = new UpdateGenreUseCase(
            new GenreRepository(actDbContext),
            unitOfWork,
            new CategoryRepository(actDbContext)
        );
        List<Guid> categoryIdsToRelate = newRelatedCategories
            .Select(category => category.Id).ToList();
        Guid invalidCategoryId = Guid.NewGuid();
        categoryIdsToRelate.Add(invalidCategoryId);
        UpdateGenreInput input = new UpdateGenreInput(
            targetGenre.Id,
            fixture.GetValidGenreName(),
            !targetGenre.IsActive,
            categoryIdsToRelate
        );

        Func<Task<GenreModelOutput>> action =
            async () => await updateGenre.Handle(
                input,
                CancellationToken.None
            );

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {invalidCategoryId}");
    }

    [Fact(DisplayName = nameof(UpdateGenreThrowsWhenNotFound))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreThrowsWhenNotFound()
    {
        List<DomainEntity.Genre> exampleGenres = fixture.GetExampleListGenres();
        CodeflixCatalogDbContext arrangeDbContext = fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContext = fixture.CreateDbContext(true);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            actDbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        UpdateGenreUseCase updateGenre = new UpdateGenreUseCase(
            new GenreRepository(actDbContext),
            unitOfWork,
            new CategoryRepository(actDbContext)
        );
        Guid randomGuid = Guid.NewGuid();
        UpdateGenreInput input = new UpdateGenreInput(
            randomGuid,
            fixture.GetValidGenreName(),
            true
        );

        Func<Task<GenreModelOutput>> action =
            async () => await updateGenre.Handle(
                input,
                CancellationToken.None
            );

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{randomGuid}' not found.");
    }
}