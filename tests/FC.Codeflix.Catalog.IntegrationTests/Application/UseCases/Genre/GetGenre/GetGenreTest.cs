using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

using FluentAssertions;

using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genres.GetGenre;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.GetGenre;

[Collection(nameof(GetGenreTestFixture))]
public class GetGenreTest
{
    private readonly GetGenreTestFixture _fixture;

    public GetGenreTest(GetGenreTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("Integration/Application", "GetGenre - Use Cases")]
    public async Task GetGenre()
    {
        var genresExampleList = _fixture.GetExampleListGenres(10);
        var expectedGenre = genresExampleList[5];
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();

        var dbActContext = _fixture.CreateDbContext(true);
        var categoryRepository = new CategoryRepository(dbArrangeContext);
        var genreRepository = new GenreRepository(dbActContext);
        var useCase = new GetGenreUseCase(genreRepository, categoryRepository);
        var input = new GetGenreInput(expectedGenre.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(expectedGenre.Id);
        output.Name.Should().Be(expectedGenre.Name);
        output.IsActive.Should().Be(expectedGenre.IsActive);
        output.CreatedAt.Should().Be(expectedGenre.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetGenreThrowsWhenNotFound))]
    [Trait("Integration/Application", "GetGenre - Use Cases")]
    public async Task GetGenreThrowsWhenNotFound()
    {
        var genresExampleList = _fixture.GetExampleListGenres(10);
        var randomGuid = Guid.NewGuid();
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var dbActContext = _fixture.CreateDbContext(true);
        var categoryRepository = new CategoryRepository(dbArrangeContext);
        var genreRepository = new GenreRepository(dbActContext);
        var useCase = new GetGenreUseCase(genreRepository, categoryRepository);
        var input = new GetGenreInput(randomGuid);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{randomGuid}' not found.");
    }

    [Fact(DisplayName = nameof(GetGenreWithCategoryRelations))]
    [Trait("Integration/Application", "GetGenre - Use Cases")]
    public async Task GetGenreWithCategoryRelations()
    {
        var genresExampleList = _fixture.GetExampleListGenres(10);
        var categoriesExampleList = _fixture.GetExampleCategoriesList(5);
        var expectedGenre = genresExampleList[5];
        categoriesExampleList.ForEach(category => expectedGenre.AddCategory(category.Id)
        );
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Categories.AddRangeAsync(categoriesExampleList);
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.GenresCategories.AddRangeAsync(
            expectedGenre.Categories.Select(categoryId => new GenresCategories(
                    expectedGenre.Id,
                    categoryId
                )
            )
        );
        await dbArrangeContext.SaveChangesAsync();
        var dbActContext = _fixture.CreateDbContext(true);
        var categoryRepository = new CategoryRepository(dbArrangeContext);
        var genreRepository = new GenreRepository(dbActContext);
        var useCase = new GetGenreUseCase(genreRepository, categoryRepository);
        var input = new GetGenreInput(expectedGenre.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(expectedGenre.Id);
        output.Name.Should().Be(expectedGenre.Name);
        output.IsActive.Should().Be(expectedGenre.IsActive);
        output.CreatedAt.Should().Be(expectedGenre.CreatedAt);
        output.Categories.Should().HaveCount(expectedGenre.Categories.Count);
        output.Categories.ToList().ForEach(relationModel =>
            {
                expectedGenre.Categories.Should().Contain(relationModel.Id);
                var relatedCategory = categoriesExampleList
                    .Find(category => category.Id == relationModel.Id);
                relationModel.Name.Should().Be(relatedCategory!.Name);
            }
        );
    }
}