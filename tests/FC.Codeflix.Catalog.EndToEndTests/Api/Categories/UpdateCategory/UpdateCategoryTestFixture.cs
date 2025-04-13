using FC.Codeflix.Catalog.Api.Models.Categories;
using FC.Codeflix.Catalog.EndToEndTests.Api.Categories.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Categories.UpdateCategory;

public class ApiTempInput
{
    public ApiTempInput(string name, string? description = null, bool? isActive = null)
    {
        Name = name;
        Description = description;
        IsActive = isActive;
    }

    public string Name { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}

[CollectionDefinition(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryTestFixture>
{
}

public class UpdateCategoryTestFixture : CategoryBaseFixture
{
    public UpdateCategoryApiInput GetExampleInput()
    {
        return new(
             GetValidCategoryName(),
             GetValidCategoryDescription(),
             GetRandomBoolean()
         );
    }
}
