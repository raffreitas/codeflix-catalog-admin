using FC.Codeflix.Catalog.EndToEndTests.Api.Genres.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genres.GetGenre;

[CollectionDefinition(nameof(GetGenreApiTestFixture))]
public class GetGenreApiTestFixtureCollection :
    ICollectionFixture<GetGenreApiTestFixture>
{ }

public class GetGenreApiTestFixture
    : GenreBaseFixture
{
    public GetGenreApiTestFixture()
        : base()
    {

    }
}
