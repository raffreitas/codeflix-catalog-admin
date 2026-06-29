using Codeflix.Catalog.Application.UseCases.Categories.ListCategories;
using Codeflix.Catalog.UnitTests.Application.Categories.Common;

using Codeflix.Catalog.Domain.Entities;
using Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

namespace Codeflix.Catalog.UnitTests.Application.Categories.ListCategories;

[CollectionDefinition(nameof(ListCategoriesTestFixtureCollection))]
public class ListCategoriesTestFixtureCollection : ICollectionFixture<ListCategoriesTestFixture>
{
}

public class ListCategoriesTestFixture : CategoryUseCasesBaseFixture
{
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
            Page: Faker.Random.Int(1, 10),
            PerPage: Faker.Random.Int(15, 100),
            Search: Faker.Commerce.ProductName(),
            Sort: Faker.Commerce.ProductName(),
            Dir: random.Next(0, 10) > 5 ? SearchOrder.Asc : SearchOrder.Desc
        );
        return input;
    }
}
