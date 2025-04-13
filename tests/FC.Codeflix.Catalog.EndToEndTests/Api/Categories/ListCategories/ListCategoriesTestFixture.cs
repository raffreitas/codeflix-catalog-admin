using FC.Codeflix.Catalog.EndToEndTests.Api.Categories.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Categories.ListCategories;

[CollectionDefinition(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTestFixtureCollection : ICollectionFixture<ListCategoriesTestFixture>
{ 
}

public class ListCategoriesTestFixture : CategoryBaseFixture
{
}
