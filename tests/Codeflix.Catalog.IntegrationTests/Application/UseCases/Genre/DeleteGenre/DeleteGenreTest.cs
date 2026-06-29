using Codeflix.Catalog.Application;
using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Application.UseCases.Genres.DeleteGenre;
using Codeflix.Catalog.Infra.Data.EF;
using Codeflix.Catalog.Infra.Data.EF.Models;
using Codeflix.Catalog.Infra.Data.EF.Repositories;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.DeleteGenre;

[Collection(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTest(DeleteGenreTestFixture fixture)
{
    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("Integration/Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenre()
    {
        var genresExampleList = fixture.GetExampleListGenres();
        var targetGenre = genresExampleList[5];
        var dbArrangeContext = fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var actDbContext = fixture.CreateDbContext(true);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            actDbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        var useCase = new DeleteGenreUseCase(
            new GenreRepository(actDbContext),
            unitOfWork
        );
        var input = new DeleteGenreInput(targetGenre.Id);

        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = fixture.CreateDbContext(true);
        var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
    [Trait("Integration/Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenreWithRelations()
    {
        var genresExampleList = fixture.GetExampleListGenres();
        var targetGenre = genresExampleList[5];
        var exampleCategories = fixture.GetExampleCategoriesList(5);
        var dbArrangeContext = fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.Categories.AddRangeAsync(exampleCategories);
        await dbArrangeContext.GenresCategories.AddRangeAsync(
            exampleCategories.Select(category =>
                new GenresCategories(
                    category.Id,
                    targetGenre.Id
                )
            )
        );
        await dbArrangeContext.SaveChangesAsync();
        var actDbContext = fixture.CreateDbContext(true);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            actDbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        var useCase = new DeleteGenreUseCase(
            new GenreRepository(actDbContext),
            unitOfWork
        );
        var input = new DeleteGenreInput(targetGenre.Id);

        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = fixture.CreateDbContext(true);
        var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().BeNull();
        var relations = await assertDbContext.GenresCategories.AsNoTracking()
            .Where(relation => relation.GenreId == targetGenre.Id)
            .ToListAsync();
        relations.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(DeleteGenreThrowsWhenNotFound))]
    [Trait("Integration/Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenreThrowsWhenNotFound()
    {
        var genresExampleList = fixture.GetExampleListGenres();
        var dbArrangeContext = fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var actDbContext = fixture.CreateDbContext(true);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            actDbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        var useCase = new DeleteGenreUseCase(
            new GenreRepository(actDbContext),
            unitOfWork
        );
        var randomGuid = Guid.NewGuid();
        var input = new DeleteGenreInput(randomGuid);

        Func<Task> action =
            async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{randomGuid}' not found.");
    }
}