using Codeflix.Catalog.EndToEndTests.Api.Genres.Common;

namespace Codeflix.Catalog.EndToEndTests.Api.Genres.GetGenre;

[CollectionDefinition(nameof(GetGenreApiTestFixture))]
public class GetGenreApiTestFixtureCollection :
    ICollectionFixture<GetGenreApiTestFixture>
{ }

public class GetGenreApiTestFixture
    : GenreBaseFixture
{
}
