using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.EndToEndTests.Api.Categories.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Categories.GetCategory;

[CollectionDefinition(nameof(GetCategoryTestFixture))]
public class GetCategoryTestFixtureCollection : ICollectionFixture<GetCategoryTestFixture> { }
public class GetCategoryTestFixture : CategoryBaseFixture
{
    public Category GetExampleCategory()
    {
        return new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());
    }

    public List<Category> GetExampleCategoriesList(int length = 10)
    {
        return [.. Enumerable.Range(1, length).Select(_ => GetExampleCategory())];
    }
}
