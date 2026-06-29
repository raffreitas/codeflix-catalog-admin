using Codeflix.Catalog.EndToEndTests.Api.Categories.Common;

namespace Codeflix.Catalog.EndToEndTests.Api.Categories.DeleteCategory;

[CollectionDefinition(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTestFixtureCollection : ICollectionFixture<DeleteCategoryTestFixture> { }

public class DeleteCategoryTestFixture : CategoryBaseFixture
{
}
