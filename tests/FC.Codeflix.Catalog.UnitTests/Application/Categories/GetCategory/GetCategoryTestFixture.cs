using FC.Codeflix.Catalog.UnitTests.Application.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.Categories.GetCategory;

[CollectionDefinition(nameof(GetCategoryTestFixture))]
public class GetCategoryTestFixtureCollection : ICollectionFixture<GetCategoryTestFixture> { }

public class GetCategoryTestFixture : CategoryUseCasesBaseFixture
{
}