using Codeflix.Catalog.IntegrationTests.Application.UseCases.Categories.Common;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Categories.DeleteCategory;

[CollectionDefinition(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTestFixtureCollection : ICollectionFixture<DeleteCategoryTestFixture>
{
}
public class DeleteCategoryTestFixture : CategoryUseCasesBaseFixture
{
}
