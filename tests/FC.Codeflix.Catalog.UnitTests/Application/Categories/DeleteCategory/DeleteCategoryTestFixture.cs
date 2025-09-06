using FC.Codeflix.Catalog.UnitTests.Application.Categories.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.Categories.DeleteCategory;

[CollectionDefinition(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTestFixtureCollection : ICollectionFixture<DeleteCategoryTestFixture>
{
}

public class DeleteCategoryTestFixture : CategoryUseCasesBaseFixture
{
}