using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Genres.CreateGenre;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.UnitTests.Application.Genres.Common;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genres.CreateGenre;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureCollection : ICollectionFixture<CreateGenreTestFixture> { }

public class CreateGenreTestFixture : GenreUseCasesBaseFixture
{
    public CreateGenreInput GetExampleInput()
        => new(GetValidGenreName(), GetRandomBoolean());

    public CreateGenreInput GetExampleInputWithCategories()
    {
        var numberOfCategories = new Random().Next(1, 10);
        var categoriesIds = Enumerable
            .Range(1, numberOfCategories)
            .Select(_ => Faker.Random.Guid())
            .ToList();

        return new(GetValidGenreName(), GetRandomBoolean(), categoriesIds);
    }

    public Mock<IGenreRepository> GetGenreRepositoryMock()
        => new();

    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();
}
