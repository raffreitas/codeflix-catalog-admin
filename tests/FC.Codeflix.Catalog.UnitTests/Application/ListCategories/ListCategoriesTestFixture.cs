using FC.Codeflix.Catalog.Application.UseCases.Categories.ListCategories;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;
using FC.Codeflix.Catalog.UnitTests.Common;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.ListCategories;

[CollectionDefinition(nameof(ListCategoriesTestFixtureCollection))]
public class ListCategoriesTestFixtureCollection : ICollectionFixture<ListCategoriesTestFixture>
{
}

public class ListCategoriesTestFixture : BaseFixture
{
    public Mock<ICategoryRepository> GetCategoryRepositoryMock() => new();
    public string GetValidCategoryName()
    {
        var categoryName = string.Empty;
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if (categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10_000)
            categoryDescription = categoryDescription[..10_000];
        return categoryDescription;
    }

    public Category GetValidCategory()
        => new(GetValidCategoryName(), GetValidCategoryDescription());

    public bool GetRandomBoolean()
        => Faker.Random.Bool();

    public Category GetExampleCategory()
        => new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean());

    public List<Category> GetExampleCategoriesList(int length = 10)
    {
        List<Category> categories = [];
        for (int index = 0; index < length; index++)
            categories.Add(GetExampleCategory());
        return categories;
    }

    public ListCategoriesInput GetExampleInput()
    {
        var random = new Random();
        var input = new ListCategoriesInput(
            page: Faker.Random.Int(1, 10),
            perPage: Faker.Random.Int(15, 100),
            search: Faker.Commerce.ProductName(),
            sort: Faker.Commerce.ProductName(),
            dir: random.Next(0, 10) > 5 ? SearchOrder.Asc : SearchOrder.Desc
        );
        return input;
    }
}
