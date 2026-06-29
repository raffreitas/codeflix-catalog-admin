using Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;

using Codeflix.Catalog.Application.UseCases.Genres.CreateGenre;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureCollection
    : ICollectionFixture<CreateGenreTestFixture>
{
}

public class CreateGenreTestFixture
    : GenreUseCasesBaseFixture
{
    public CreateGenreInput GetExampleInput()
        => new CreateGenreInput(
            GetValidGenreName(),
            GetRandomBoolean()
        );
}