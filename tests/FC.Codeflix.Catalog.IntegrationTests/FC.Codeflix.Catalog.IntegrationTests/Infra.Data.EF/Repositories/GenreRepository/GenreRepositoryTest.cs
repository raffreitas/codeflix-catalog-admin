using FC.Codeflix.Catalog.Infra.Data.EF;

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
}
