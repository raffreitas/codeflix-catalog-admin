using Codeflix.Catalog.EndToEndTests.Api.Genres.Common;

namespace Codeflix.Catalog.EndToEndTests.Api.Genres.DeleteGenre;

[CollectionDefinition(nameof(DeleteGenreTestApiFixture))]
public class DeleteGenreTestApiFixtureCollection
    : ICollectionFixture<DeleteGenreTestApiFixture>
{ }

public class DeleteGenreTestApiFixture
    : GenreBaseFixture
{
}
