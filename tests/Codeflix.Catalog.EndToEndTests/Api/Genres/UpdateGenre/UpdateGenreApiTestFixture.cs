using Codeflix.Catalog.EndToEndTests.Api.Genres.Common;

namespace Codeflix.Catalog.EndToEndTests.Api.Genres.UpdateGenre;

[CollectionDefinition(nameof(UpdateGenreApiTestFixture))]
public class UpdateGenreApiTestFixtureCollection
    : ICollectionFixture<UpdateGenreApiTestFixture>
{}

public class UpdateGenreApiTestFixture
    : GenreBaseFixture
{
}
