using Codeflix.Catalog.EndToEndTests.Api.Categories.Common;

using Codeflix.Catalog.Application.UseCases.Categories.Common;

namespace Codeflix.Catalog.EndToEndTests.Api.Categories.GetCategory;

public class GetCategoryResponse
{
    public CategoryModelOutput Data { get; set; } = null!;
}

[CollectionDefinition(nameof(GetCategoryTestFixture))]
public class GetCategoryTestFixtureCollection : ICollectionFixture<GetCategoryTestFixture> { }
public class GetCategoryTestFixture : CategoryBaseFixture
{
}
