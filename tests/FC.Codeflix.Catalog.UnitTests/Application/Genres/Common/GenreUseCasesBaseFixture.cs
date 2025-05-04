using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.UnitTests.Common;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genres.Common;
public class GenreUseCasesBaseFixture : BaseFixture
{
    public string GetValidGenreName()
        => Faker.Commerce.ProductName();

    public Genre GetExampleGenre(bool? isActive = null)
        => new(GetValidGenreName(), isActive ?? GetRandomBoolean());

    public Mock<IGenreRepository> GetGenreRepositoryMock() => new();
    public Mock<ICategoryRepository> GetCategoryRepositoryMock() => new();
    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();
}
