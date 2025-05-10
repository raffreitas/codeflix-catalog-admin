using FC.Codeflix.Catalog.UnitTests.Application.Genres.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genres.DeleteGenre;

[CollectionDefinition(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTestFixtureCollection : ICollectionFixture<DeleteGenreTestFixture>
{
}

public class DeleteGenreTestFixture : GenreUseCasesBaseFixture
{
}
