using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.UnitTests.Common;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.Genres;

[CollectionDefinition(nameof(GenreTestFixture))]
public class GenreTestFixtureCollection : ICollectionFixture<GenreTestFixture> { }

public class GenreTestFixture : BaseFixture
{
    public string GetValidName()
        => Faker.Commerce.ProductName();

    public Genre GetExampleGenre(bool isActive = true, List<Guid>? categoriesIds = null)
    {
        var genre = new Genre(GetValidName(), isActive);
        if (categoriesIds is not null)
            foreach (var id in categoriesIds)
                genre.AddCategory(id);

        return genre;
    }
}
