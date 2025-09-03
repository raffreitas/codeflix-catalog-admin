using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;
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
        genreCategoriesRelation.Should().HaveCount(categoriesListExample.Count);
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
        genreFromRepository.Categories.Should().HaveCount(categoriesListExample.Count);
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

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Delete()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleGenre = fixture.GetExampleGenre();
        var categoriesListExample = fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
            await dbContext.GenresCategories.AddAsync(new GenresCategories(exampleGenre.Id, categoryId));
        await dbContext.SaveChangesAsync();
        var repositoryDbContext = fixture.CreateDbContext(preserveData: true);
        var genreRepository = new Repository.GenreRepository(repositoryDbContext);

        await genreRepository.Delete(exampleGenre, CancellationToken.None);
        await repositoryDbContext.SaveChangesAsync();

        var assertsDbContext = fixture.CreateDbContext(preserveData: true);
        var dbGenre = await assertsDbContext
            .Genres
            .AsNoTracking()
            .FirstOrDefaultAsync((x) => x.Id == exampleGenre.Id);
        dbGenre.Should().BeNull();
        var genreCategoriesRelation = await assertsDbContext
            .GenresCategories
            .Where(x => x.GenreId == exampleGenre.Id)
            .Select(x => x.CategoryId)
            .ToListAsync();
        genreCategoriesRelation.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Update()
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
        var repositoryDbContext = fixture.CreateDbContext(preserveData: true);
        var genreRepository = new Repository.GenreRepository(repositoryDbContext);

        exampleGenre.Update(fixture.GetValidGenreName());
        if (exampleGenre.IsActive) exampleGenre.Deactivate();
        else exampleGenre.Activate();
        await genreRepository.Update(exampleGenre, CancellationToken.None);
        await repositoryDbContext.SaveChangesAsync();

        var assertsDbContext = fixture.CreateDbContext(preserveData: true);
        var genreFromDb = await assertsDbContext
            .Genres
            .AsNoTracking()
            .FirstOrDefaultAsync((x) => x.Id == exampleGenre.Id);

        genreFromDb.Should().NotBeNull();
        genreFromDb.Name.Should().Be(exampleGenre.Name);
        genreFromDb.IsActive.Should().Be(exampleGenre.IsActive);
        genreFromDb.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelation = await assertsDbContext
            .GenresCategories
            .Where(x => x.GenreId == exampleGenre.Id)
            .ToListAsync(CancellationToken.None);
        genreCategoriesRelation.Should().NotBeNull();
        genreCategoriesRelation.Should().HaveCount(categoriesListExample.Count);
        genreCategoriesRelation.ForEach(relation =>
        {
            var expectedCategory = categoriesListExample.FirstOrDefault(x => x.Id == relation.CategoryId);
            expectedCategory.Should().NotBeNull();
            relation.CategoryId.Should().Be(expectedCategory.Id);
        });
    }

    [Fact(DisplayName = nameof(UpdateRemovingRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task UpdateRemovingRelations()
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
        var repositoryDbContext = fixture.CreateDbContext(preserveData: true);
        var genreRepository = new Repository.GenreRepository(repositoryDbContext);

        exampleGenre.Update(fixture.GetValidGenreName());
        if (exampleGenre.IsActive) exampleGenre.Deactivate();
        else exampleGenre.Activate();
        exampleGenre.RemoveAllCategories();
        await genreRepository.Update(exampleGenre, CancellationToken.None);
        await repositoryDbContext.SaveChangesAsync();

        var assertsDbContext = fixture.CreateDbContext(preserveData: true);
        var genreFromDb = await assertsDbContext
            .Genres
            .AsNoTracking()
            .FirstOrDefaultAsync((x) => x.Id == exampleGenre.Id);

        genreFromDb.Should().NotBeNull();
        genreFromDb.Name.Should().Be(exampleGenre.Name);
        genreFromDb.IsActive.Should().Be(exampleGenre.IsActive);
        genreFromDb.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelation = await assertsDbContext
            .GenresCategories
            .Where(x => x.GenreId == exampleGenre.Id)
            .ToListAsync(CancellationToken.None);
        genreCategoriesRelation.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(UpdateReplacingRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task UpdateReplacingRelations()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleGenre = fixture.GetExampleGenre();
        var categoriesListExample = fixture.GetExampleCategoriesList(3);
        var updateCategoriesListExample = fixture.GetExampleCategoriesList(2);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Categories.AddRangeAsync(updateCategoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
            await dbContext.GenresCategories.AddAsync(new GenresCategories(exampleGenre.Id, categoryId));
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repositoryDbContext = fixture.CreateDbContext(preserveData: true);
        var genreRepository = new Repository.GenreRepository(repositoryDbContext);

        exampleGenre.Update(fixture.GetValidGenreName());
        if (exampleGenre.IsActive) exampleGenre.Deactivate();
        else exampleGenre.Activate();
        exampleGenre.RemoveAllCategories();
        updateCategoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await genreRepository.Update(exampleGenre, CancellationToken.None);
        await repositoryDbContext.SaveChangesAsync();

        var assertsDbContext = fixture.CreateDbContext(preserveData: true);
        var genreFromDb = await assertsDbContext
            .Genres
            .AsNoTracking()
            .FirstOrDefaultAsync((x) => x.Id == exampleGenre.Id);

        genreFromDb.Should().NotBeNull();
        genreFromDb.Name.Should().Be(exampleGenre.Name);
        genreFromDb.IsActive.Should().Be(exampleGenre.IsActive);
        genreFromDb.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelation = await assertsDbContext
            .GenresCategories
            .Where(x => x.GenreId == exampleGenre.Id)
            .ToListAsync(CancellationToken.None);
        genreCategoriesRelation.Should().NotBeNull();
        genreCategoriesRelation.Should().HaveCount(updateCategoriesListExample.Count);
        genreCategoriesRelation.ForEach(relation =>
        {
            var expectedCategory = updateCategoriesListExample.FirstOrDefault(x => x.Id == relation.CategoryId);
            expectedCategory.Should().NotBeNull();
            relation.CategoryId.Should().Be(expectedCategory.Id);
        });
    }

    [Fact(DisplayName = nameof(SearchReturnsItemsAndTotal))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnsItemsAndTotal()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleGenresList = fixture.GetExampleGenresList(10);

        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repositoryDbContext = fixture.CreateDbContext(preserveData: true);
        var genreRepository = new Repository.GenreRepository(repositoryDbContext);

        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        var searchOutput = await genreRepository.Search(searchInput, CancellationToken.None);

        searchOutput.Should().NotBeNull();
        searchOutput.CurrentPage.Should().Be(searchInput.Page);
        searchOutput.PerPage.Should().Be(searchInput.PerPage);
        searchOutput.Total.Should().Be(exampleGenresList.Count);
        searchOutput.Items.Should().HaveCount(exampleGenresList.Count);
        foreach (var item in searchOutput.Items)
        {
            var exampleGenre = exampleGenresList.Find(x => x.Id == item.Id);
            exampleGenre.Should().NotBeNull();
            exampleGenre.Name.Should().Be(exampleGenre.Name);
            exampleGenre.IsActive.Should().Be(exampleGenre.IsActive);
            exampleGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        }
    }

    [Fact(DisplayName = nameof(SearchReturnsRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnsRelations()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleGenresList = fixture.GetExampleGenresList(10);
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        var random = new Random();
        exampleGenresList.ForEach(exampleGenre =>
        {
            var categoriesListToRelation = fixture
                .GetExampleCategoriesList(random.Next(0, 4));
            if (categoriesListToRelation.Count > 0)
            {
                categoriesListToRelation.ForEach(category => exampleGenre.AddCategory(category.Id));
                dbContext.Categories.AddRange(categoriesListToRelation);
                var relationsToAdd = categoriesListToRelation
                    .Select(category => new GenresCategories(exampleGenre.Id, category.Id))
                    .ToList();
                dbContext.GenresCategories.AddRange(relationsToAdd);
            }
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var actDbContext = fixture.CreateDbContext(preserveData: true);
        var genreRepository = new Repository.GenreRepository(actDbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        var searchOutput = await genreRepository.Search(searchInput, CancellationToken.None);

        searchOutput.Should().NotBeNull();
        searchOutput.CurrentPage.Should().Be(searchInput.Page);
        searchOutput.PerPage.Should().Be(searchInput.PerPage);
        searchOutput.Total.Should().Be(exampleGenresList.Count);
        searchOutput.Items.Should().HaveCount(exampleGenresList.Count);
        foreach (var item in searchOutput.Items)
        {
            var exampleGenre = exampleGenresList.Find(x => x.Id == item.Id);
            exampleGenre.Should().NotBeNull();
            exampleGenre.Name.Should().Be(exampleGenre.Name);
            exampleGenre.IsActive.Should().Be(exampleGenre.IsActive);
            exampleGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            exampleGenre.Categories.Should().HaveCount(exampleGenre.Categories.Count);
            exampleGenre.Categories.Should().BeEquivalentTo(exampleGenre.Categories);
        }
    }

    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnsEmptyWhenPersistenceIsEmpty()
    {
        var actDbContext = fixture.CreateDbContext(preserveData: true);
        var genreRepository = new Repository.GenreRepository(actDbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        var searchOutput = await genreRepository.Search(searchInput, CancellationToken.None);

        searchOutput.Should().NotBeNull();
        searchOutput.CurrentPage.Should().Be(searchInput.Page);
        searchOutput.PerPage.Should().Be(searchInput.PerPage);
        searchOutput.Total.Should().Be(0);
        searchOutput.Items.Should().HaveCount(0);
    }

    [Theory(DisplayName = nameof(SearchReturnsPagineted))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPagineted(
        int quantityToGenerate,
        int page,
        int perPage,
        int expectedTotal
    )
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleGenresList = fixture.GetExampleGenresList(quantityToGenerate);
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        var random = new Random();
        exampleGenresList.ForEach(exampleGenre =>
        {
            var categoriesListToRelation = fixture
                .GetExampleCategoriesList(random.Next(0, 4));
            if (categoriesListToRelation.Count > 0)
            {
                categoriesListToRelation.ForEach(category => exampleGenre.AddCategory(category.Id));
                dbContext.Categories.AddRange(categoriesListToRelation);
                var relationsToAdd = categoriesListToRelation
                    .Select(category => new GenresCategories(exampleGenre.Id, category.Id))
                    .ToList();
                dbContext.GenresCategories.AddRange(relationsToAdd);
            }
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var actDbContext = fixture.CreateDbContext(preserveData: true);
        var genreRepository = new Repository.GenreRepository(actDbContext);
        var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.Asc);

        var searchOutput = await genreRepository.Search(searchInput, CancellationToken.None);

        searchOutput.Should().NotBeNull();
        searchOutput.CurrentPage.Should().Be(searchInput.Page);
        searchOutput.PerPage.Should().Be(searchInput.PerPage);
        searchOutput.Total.Should().Be(exampleGenresList.Count);
        searchOutput.Items.Should().HaveCount(expectedTotal);
        foreach (var item in searchOutput.Items)
        {
            var exampleGenre = exampleGenresList.Find(x => x.Id == item.Id);
            exampleGenre.Should().NotBeNull();
            exampleGenre.Name.Should().Be(exampleGenre.Name);
            exampleGenre.IsActive.Should().Be(exampleGenre.IsActive);
            exampleGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            exampleGenre.Categories.Should().HaveCount(exampleGenre.Categories.Count);
            exampleGenre.Categories.Should().BeEquivalentTo(exampleGenre.Categories);
        }
    }
}