using Codeflix.Catalog.EndToEndTests.Api.Categories.Common;

using Codeflix.Catalog.Application.UseCases.Categories.CreateCategory;

namespace Codeflix.Catalog.EndToEndTests.Api.Categories.CreateCategory;

[CollectionDefinition(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTestFixtureCollection : ICollectionFixture<CreateCategoryTestFixture>
{ }
public class CreateCategoryTestFixture : CategoryBaseFixture
{
    public CreateCategoryInput GetExampleInput()
    {
        return new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );
    }
}
