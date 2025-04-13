using System.Net;

using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Categories.GetCategory;

[Collection(nameof(GetCategoryTestFixture))]
public class GetCategoryTest(GetCategoryTestFixture fixture)
    : IDisposable
{
    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("E2E/API", "Category/Get - Endpoints")]
    public async Task GetCategory()
    {
        var exampleCategoriesList = fixture.GetExampleCategoriesList(20);
        await fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];

        var (response, output) = await fixture
            .ApiClient
            .Get<CategoryModelOutput>($"/categories/{exampleCategory.Id}");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(exampleCategory.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(ErrorWhenCategoryNotFound))]
    [Trait("E2E/API", "Category/Get - Endpoints")]
    public async Task ErrorWhenCategoryNotFound()
    {
        var exampleCategoriesList = fixture.GetExampleCategoriesList(20);
        await fixture.Persistence.InsertList(exampleCategoriesList);
        var randomGuid = Guid.NewGuid();

        var (response, output) = await fixture
            .ApiClient
            .Get<ProblemDetails>($"/categories/{randomGuid}");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output.Title.Should().Be("Not Found");
        output.Type.Should().Be("NotFound");
        output.Status.Should().Be((int)HttpStatusCode.NotFound);
        output.Detail.Should().Be($"Category '{randomGuid}' not found.");
    }

    public void Dispose() => fixture.CleanPersistence();
}