using FC.Codeflix.Catalog.Domain.Entities;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.Categories;

public class CategoryTestFixture
{
    public Category GetValidCategory() 
        => new("Category Name", "Category Description");
}

[CollectionDefinition(nameof(CategoryTestFixture))]
public class CategoryTestFixtureCollection : ICollectionFixture<CategoryTestFixture>
{
}