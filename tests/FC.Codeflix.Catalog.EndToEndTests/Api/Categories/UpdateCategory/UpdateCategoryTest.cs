using System.Net;

using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;
using FC.Codeflix.Catalog.Application.UseCases.Categories.UpdateCategory;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Categories.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest(UpdateCategoryTestFixture fixture)
    : IDisposable
{
    [Fact(DisplayName = nameof(UpdateCategory))]
    [Trait("E2E/API", "Category/Update - Endpoints")]
    public async Task UpdateCategory()
    {
        var exampleCategoriesList = fixture.GetExampleCategoriesList(20);
        await fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];
        var input = fixture.GetExampleInput(exampleCategory.Id);

        var (response, output) = await fixture.ApiClient
            .Put<CategoryModelOutput>($"/categories/{exampleCategory.Id}", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)input.IsActive!);

        var dbCategory = await fixture.Persistence
            .GetById(exampleCategory.Id);

        dbCategory.Should().NotBeNull();
        dbCategory.Id.Should().Be(output.Id);
        dbCategory.Name.Should().Be(output.Name);
        dbCategory.Description.Should().Be(output.Description);
        dbCategory.IsActive.Should().Be(output.IsActive);
    }

    [Fact(DisplayName = nameof(UpdateOnlyNameCategory))]
    [Trait("E2E/API", "Category/Update - Endpoints")]
    public async Task UpdateOnlyNameCategory()
    {
        var exampleCategoriesList = fixture.GetExampleCategoriesList(20);
        await fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];
        var input = new UpdateCategoryInput(exampleCategory.Id, fixture.GetValidCategoryName());

        var (response, output) = await fixture.ApiClient
            .Put<CategoryModelOutput>($"/categories/{exampleCategory.Id}", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);

        var dbCategory = await fixture.Persistence
            .GetById(exampleCategory.Id);

        dbCategory.Should().NotBeNull();
        dbCategory.Id.Should().Be(output.Id);
        dbCategory.Name.Should().Be(output.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
    }


    [Fact(DisplayName = nameof(UpdateCategoryNameAndDescription))]
    [Trait("E2E/API", "Category/Update - Endpoints")]
    public async Task UpdateCategoryNameAndDescription()
    {
        var exampleCategoriesList = fixture.GetExampleCategoriesList(20);
        await fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];
        var input = new UpdateCategoryInput(
            exampleCategory.Id,
            fixture.GetValidCategoryName(),
            fixture.GetValidCategoryDescription());

        var (response, output) = await fixture.ApiClient
            .Put<CategoryModelOutput>($"/categories/{exampleCategory.Id}", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive!);

        var dbCategory = await fixture.Persistence
            .GetById(exampleCategory.Id);

        dbCategory.Should().NotBeNull();
        dbCategory.Id.Should().Be(output.Id);
        dbCategory.Name.Should().Be(output.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
    }

    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    [Trait("E2E/API", "Category/Update - Endpoints")]
    public async Task ErrorWhenNotFound()
    {
        var exampleCategoriesList = fixture.GetExampleCategoriesList();
        await fixture.Persistence.InsertList(exampleCategoriesList);
        var randomGuid = Guid.NewGuid();
        var input = fixture.GetExampleInput(randomGuid);

        var (response, output) = await fixture.ApiClient
            .Put<ProblemDetails>($"/categories/{randomGuid}", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output.Title.Should().Be("Not Found");
        output.Type.Should().Be("NotFound");
        output.Status.Should().Be((int)HttpStatusCode.NotFound);
        output.Detail.Should().Be($"Category '{randomGuid}' not found.");
    }

    [Theory(DisplayName = nameof(ErrorWhenCantInstantiateAggregate))]
    [Trait("E2E/API", "Category/Update - Endpoints")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs),
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async Task ErrorWhenCantInstantiateAggregate(UpdateCategoryInput input, string expectedDetail)
    {
        var exampleCategoriesList = fixture.GetExampleCategoriesList(20);
        await fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];
        input.Id = exampleCategory.Id;

        var (response, output) = await fixture.ApiClient
            .Put<ProblemDetails>($"/categories/{exampleCategory.Id}", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output.Title.Should().Be("One or more validation error occurred");
        output.Type.Should().Be("UnprocessableEntity");
        output.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        output.Detail.Should().Be(expectedDetail);
    }

    public void Dispose() => fixture.CleanPersistence();
}
