using Bogus;

using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.IntegrationTests.Base;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository;

[CollectionDefinition(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTestFixtureCollection : ICollectionFixture<GenreRepositoryTestFixture>
{
}
public class GenreRepositoryTestFixture : BaseFixture
{
    public string GetValidGenreName() => Faker.Commerce.ProductName();

    public bool GetRandomBoolean() => Faker.Random.Bool();

    public Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null)
    {
        var genre = new Genre(GetValidGenreName(), isActive ?? GetRandomBoolean());
        categoriesIds?.ForEach(genre.AddCategory);
        return genre;
    }

    public string GetValidCategoryName()
    {
        var categoryName = string.Empty;
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if (categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10_000)
            categoryDescription = categoryDescription[..10_000];
        return categoryDescription;
    }

    public Category GetExampleCategory()
      => new(
          GetValidCategoryName(),
          GetValidCategoryDescription(),
          GetRandomBoolean());

    public List<Category> GetExampleCategoriesList(int length = 10)
      => [.. Enumerable.Range(0, length).Select(_ => GetExampleCategory())];
}
