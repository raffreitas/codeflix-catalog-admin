using Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.GetGenre;

[CollectionDefinition(nameof(GetGenreTestFixture))]
public class GetGenreTestFixtureCollection
    : ICollectionFixture<GetGenreTestFixture>
{ }
public class GetGenreTestFixture
    : GenreUseCasesBaseFixture
{

}
