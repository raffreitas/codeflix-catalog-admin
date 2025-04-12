using FC.Codeflix.Catalog.Application.UseCases.Categories.UpdateCategory;
using FC.Codeflix.Catalog.EndToEndTests.Api.Categories.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Categories.UpdateCategory;

[CollectionDefinition(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryTestFixture>
{
}

public class UpdateCategoryTestFixture : CategoryBaseFixture
{
    public UpdateCategoryInput GetExampleInput(Guid? id = null)
    {
        return new(
            id ?? Guid.NewGuid(),
             GetValidCategoryName(),
             GetValidCategoryDescription(),
             GetRandomBoolean()
         );
    }
}
