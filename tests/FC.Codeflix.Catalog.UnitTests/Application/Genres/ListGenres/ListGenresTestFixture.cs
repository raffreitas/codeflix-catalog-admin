using FC.Codeflix.Catalog.Application.UseCases.Genres.ListGenres;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;
using FC.Codeflix.Catalog.UnitTests.Application.Genres.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genres.ListGenres;

[CollectionDefinition(nameof(ListGenresTestFixture))]
public class ListGenreTestFixtureCollection : ICollectionFixture<ListGenresTestFixture>
{
}

public class ListGenresTestFixture : GenreUseCasesBaseFixture
{
    public ListGenresInput GetExampleInput()
    {
        var random = new Random();
        var input = new ListGenresInput(
            page: Faker.Random.Int(1, 10),
            perPage: Faker.Random.Int(15, 100),
            search: Faker.Commerce.ProductName(),
            sort: Faker.Commerce.ProductName(),
            dir: random.Next(0, 10) > 5 ? SearchOrder.Asc : SearchOrder.Desc
        );
        return input;
    }
}
