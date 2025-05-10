using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;


namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository;

[Collection(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTest(GenreRepositoryTestFixture fixture)
{
    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Insert()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleGenre = fixture.GetExampleGenre();
        var categoriesListExample = fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);

        var genreRepository = new Repository.GenreRepository(dbContext);

        await genreRepository.Insert(exampleGenre, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = fixture.CreateDbContext(preserveData: true);
        var dbGenre = await assertsDbContext
            .Genres
            .SingleAsync((x) => x.Id == exampleGenre.Id, CancellationToken.None);

        dbGenre.Should().NotBeNull();
        dbGenre.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelation = await assertsDbContext
            .GenresCategories
            .Where(x => x.GenreId == exampleGenre.Id)
            .ToListAsync(CancellationToken.None);
        genreCategoriesRelation.Should().NotBeNull();
        genreCategoriesRelation.Should().HaveCount(3);
        genreCategoriesRelation.ForEach(relation =>
        {
            var expectedCategory = categoriesListExample.FirstOrDefault(x => x.Id == relation.CategoryId);
            expectedCategory.Should().NotBeNull();
            relation.CategoryId.Should().Be(expectedCategory.Id);
        });
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Get()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleGenre = fixture.GetExampleGenre();
        var categoriesListExample = fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
            await dbContext.GenresCategories.AddAsync(new GenresCategories(exampleGenre.Id, categoryId));
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var genreRepository = new Repository.GenreRepository(fixture.CreateDbContext(preserveData: true));

        var genreFromRepository = await genreRepository.Get(exampleGenre.Id, CancellationToken.None);

        genreFromRepository.Should().NotBeNull();
        genreFromRepository.Name.Should().Be(exampleGenre.Name);
        genreFromRepository.IsActive.Should().Be(exampleGenre.IsActive);
        genreFromRepository.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        genreFromRepository.Categories.Should().NotBeNull();
        genreFromRepository.Categories.Should().HaveCount(3);
        genreFromRepository.Categories.ToList().ForEach(categoryId =>
        {
            var expectedCategory = categoriesListExample.FirstOrDefault(x => x.Id == categoryId);
            expectedCategory.Should().NotBeNull();
            categoryId.Should().Be(expectedCategory.Id);
        });
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task ThrowWhenNotFound()
    {
        var exampleId = Guid.NewGuid();
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleGenre = fixture.GetExampleGenre();
        var categoriesListExample = fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
            await dbContext.GenresCategories.AddAsync(new GenresCategories(exampleGenre.Id, categoryId));
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var genreRepository = new Repository.GenreRepository(fixture.CreateDbContext(preserveData: true));

        var act = async () => await genreRepository.Get(exampleId, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{exampleId}' not found.");
    }
}
