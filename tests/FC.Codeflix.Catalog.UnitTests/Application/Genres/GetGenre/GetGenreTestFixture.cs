using FC.Codeflix.Catalog.UnitTests.Application.Genres.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genres.GetGenre;

[CollectionDefinition(nameof(GetGenreTestFixture))]
public class GetGenreTestFixtureCollection : ICollectionFixture<GetGenreTestFixture>
{
}

public class GetGenreTestFixture : GenreUseCasesBaseFixture
{
}
