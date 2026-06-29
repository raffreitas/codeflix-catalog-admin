using Codeflix.Catalog.Application.UseCases.Genres.ListGenres;
using Codeflix.Catalog.UnitTests.Application.Genres.Common;

using Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

namespace Codeflix.Catalog.UnitTests.Application.Genres.ListGenres;

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
            Page: Faker.Random.Int(1, 10),
            PerPage: Faker.Random.Int(15, 100),
            Search: Faker.Commerce.ProductName(),
            Sort: Faker.Commerce.ProductName(),
            Dir: random.Next(0, 10) > 5 ? SearchOrder.Asc : SearchOrder.Desc
        );
        return input;
    }
}
