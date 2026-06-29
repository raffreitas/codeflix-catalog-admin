using Codeflix.Catalog.UnitTests.Application.Genres.Common;

namespace Codeflix.Catalog.UnitTests.Application.Genres.DeleteGenre;

[CollectionDefinition(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTestFixtureCollection : ICollectionFixture<DeleteGenreTestFixture>
{
}

public class DeleteGenreTestFixture : GenreUseCasesBaseFixture
{
}
