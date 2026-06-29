using Codeflix.Catalog.IntegrationTests.Application.UseCases.Categories.Common;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Categories.GetCategory;

[CollectionDefinition(nameof(GetCategoryTestFixture))]
public class GetCategoryTestFixtureCollection : ICollectionFixture<GetCategoryTestFixture>
{
}

public class GetCategoryTestFixture : CategoryUseCasesBaseFixture
{
}
