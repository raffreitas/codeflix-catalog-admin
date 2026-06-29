using Codeflix.Catalog.UnitTests.Application.Genres.Common;

namespace Codeflix.Catalog.UnitTests.Application.Genres.GetGenre;

[CollectionDefinition(nameof(GetGenreTestFixture))]
public class GetGenreTestFixtureCollection : ICollectionFixture<GetGenreTestFixture>
{
}

public class GetGenreTestFixture : GenreUseCasesBaseFixture
{
}
