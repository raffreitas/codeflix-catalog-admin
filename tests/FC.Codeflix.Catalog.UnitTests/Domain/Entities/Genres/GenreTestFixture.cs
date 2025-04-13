using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.UnitTests.Common;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.Genres;

[CollectionDefinition(nameof(GenreTestFixture))]
public class GenreTestFixtureCollection : ICollectionFixture<GenreTestFixture> { }

public class GenreTestFixture : BaseFixture
{
    public string GetValidName()
        => Faker.Commerce.ProductName();

    public Genre GetExampleGenre(bool isActive = true)
        => new(GetValidName(), isActive);
}
