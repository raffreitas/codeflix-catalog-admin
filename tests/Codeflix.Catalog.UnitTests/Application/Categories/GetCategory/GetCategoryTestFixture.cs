using Codeflix.Catalog.UnitTests.Application.Categories.Common;

namespace Codeflix.Catalog.UnitTests.Application.Categories.GetCategory;

[CollectionDefinition(nameof(GetCategoryTestFixture))]
public class GetCategoryTestFixtureCollection : ICollectionFixture<GetCategoryTestFixture> { }

public class GetCategoryTestFixture : CategoryUseCasesBaseFixture
{
}