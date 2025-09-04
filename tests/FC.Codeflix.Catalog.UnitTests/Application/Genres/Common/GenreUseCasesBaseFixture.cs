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

    public Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null)
    {
        var genre = new Genre(GetValidGenreName(), isActive ?? GetRandomBoolean());
        categoriesIds?.ForEach(genre.AddCategory);
        return genre;
    }

    public List<Genre> GetExampleGenresList(int count = 10)
    {
        return Enumerable.Range(1, count).Select(_ =>
        {
            var genre = new Genre(GetValidGenreName(), GetRandomBoolean());
            GetRandomIdsList().ForEach(genre.AddCategory);
            return genre;
        }).ToList();
    }

    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if (categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription =
            Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10_000)
            categoryDescription =
                categoryDescription[..10_000];
        return categoryDescription;
    }

    public Category GetExampleCategory()
        => new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );

    public List<Category> GetExampleCategoriesList(int count = 5)
        => Enumerable.Range(0, count).Select(_ => GetExampleCategory())
            .ToList();

    public List<Guid> GetRandomIdsList(int? count = null)
        => [.. Enumerable.Range(1, count ?? new Random().Next(1, 10)).Select(_ => Guid.NewGuid())];

    public Mock<IGenreRepository> GetGenreRepositoryMock() => new();
    public Mock<ICategoryRepository> GetCategoryRepositoryMock() => new();
    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();
}