using System.Net;

using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;
using FC.Codeflix.Catalog.Application.UseCases.Categories.CreateCategory;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Categories.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest(CreateCategoryTestFixture fixture)
    : IDisposable
{
    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("E2E/API", "Category/Create - Endpoints")]
    public async Task CreateCategory()
    {
        var input = fixture.GetExampleInput();

        var (response, output) = await fixture
            .ApiClient
            .Post<CategoryModelOutput>("/categories", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default);

        var dbCategory = await fixture.Persistence
            .GetById(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory.Id.Should().Be(output.Id);
        dbCategory.Name.Should().Be(output.Name);
        dbCategory.Description.Should().Be(output.Description);
        dbCategory.IsActive.Should().Be(output.IsActive);
        dbCategory.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(ErrorWhenCantInstantiateAggregate))]
    [Trait("E2E/API", "Category/Create - Endpoints")]
    [MemberData(
        nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
        MemberType = typeof(CreateCategoryTestDataGenerator)
    )]
    public async Task ErrorWhenCantInstantiateAggregate(CreateCategoryInput input, string expectedDetail)
    {
        var (response, output) = await fixture
            .ApiClient
            .Post<ProblemDetails>("/categories", input);

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
