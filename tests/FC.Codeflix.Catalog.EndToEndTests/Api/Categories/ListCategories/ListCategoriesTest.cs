using System.Net;

using FC.Codeflix.Catalog.Application.UseCases.Categories.ListCategories;

using FluentAssertions;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Categories.ListCategories;

[Collection(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTest(ListCategoriesTestFixture fixture) 
    : IDisposable
{
    [Fact(DisplayName = nameof(ListCategoriesAndTotalByDefault))]
    [Trait("E2E/API", "Category/List - Endpoints")]
    public async Task ListCategoriesAndTotalByDefault()
    {
        var defaultPerPage = 15;
        var exampleCategoriesList = fixture.GetExampleCategoriesList(20);
        await fixture.Persistence.InsertList(exampleCategoriesList);

        var (response, output) = await fixture.ApiClient
            .Get<ListCategoriesOutput>($"/categories");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Total.Should().Be(exampleCategoriesList.Count);
        output.Page.Should().Be(1);
        output.PerPage.Should().Be(defaultPerPage);
        output.Items.Should().HaveCount(defaultPerPage);
        foreach (var item in output.Items)
        {
            var expectedItem = exampleCategoriesList.Single(x => x.Id == item.Id);

            item.Should().NotBeNull();
            item.Id.Should().Be(expectedItem.Id);
            item.Name.Should().Be(expectedItem.Name);
            item.Description.Should().Be(expectedItem.Description);
            item.IsActive.Should().Be(expectedItem.IsActive);
            item.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }

    public void Dispose() => fixture.CleanPersistence();
}
