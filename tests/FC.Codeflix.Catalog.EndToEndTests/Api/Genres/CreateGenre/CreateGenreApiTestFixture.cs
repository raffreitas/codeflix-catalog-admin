using FC.Codeflix.Catalog.EndToEndTests.Api.Genres.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genres.CreateGenre;

[CollectionDefinition(nameof(CreateGenreApiTestFixture))]
public class CreateGenreApiTestFixtureCollection
    : ICollectionFixture<CreateGenreApiTestFixture>
{ }

public class CreateGenreApiTestFixture
    : GenreBaseFixture
{
}
