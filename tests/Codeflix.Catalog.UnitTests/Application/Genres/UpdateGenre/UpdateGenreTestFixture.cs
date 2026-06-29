using Codeflix.Catalog.UnitTests.Application.Genres.Common;

namespace Codeflix.Catalog.UnitTests.Application.Genres.UpdateGenre;

[CollectionDefinition(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTestFixtureCollection : ICollectionFixture<UpdateGenreTestFixture> { }

public class UpdateGenreTestFixture : GenreUseCasesBaseFixture
{
}
