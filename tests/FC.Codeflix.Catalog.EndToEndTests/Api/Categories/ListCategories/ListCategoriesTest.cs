using System.Net;

using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;
using FC.Codeflix.Catalog.Application.UseCases.Categories.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Extensions;
using FC.Codeflix.Catalog.EndToEndTests.Models;

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
            .Get<TestApiResponseList<CategoryModelOutput>>($"/categories");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().NotBeEmpty();
        output.Meta.Should().NotBeNull();
        output.Meta.Total.Should().Be(exampleCategoriesList.Count);
        output.Meta.CurrentPage.Should().Be(1);
        output.Meta.PerPage.Should().Be(defaultPerPage);
        output.Data.Should().HaveCount(defaultPerPage);
        foreach (var item in output.Data)
        {
            var expectedItem = exampleCategoriesList.Single(x => x.Id == item.Id);

            item.Should().NotBeNull();
            item.Id.Should().Be(expectedItem.Id);
            item.Name.Should().Be(expectedItem.Name);
            item.Description.Should().Be(expectedItem.Description);
            item.IsActive.Should().Be(expectedItem.IsActive);
            item.CreatedAt.TrimMilliseconds().Should().Be(expectedItem.CreatedAt.TrimMilliseconds());
        }
    }

    [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
    [Trait("E2E/API", "Category/List - Endpoints")]
    public async Task ItemsEmptyWhenPersistenceEmpty()
    {
        var (response, output) = await fixture.ApiClient
            .Get<TestApiResponseList<CategoryModelOutput>>($"/categories");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.Total.Should().Be(0);
        output.Data.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(ListCategoriesAndTotal))]
    [Trait("E2E/API", "Category/List - Endpoints")]
    public async Task ListCategoriesAndTotal()
    {
        var exampleCategoriesList = fixture.GetExampleCategoriesList(20);
        await fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput(page: 1, perPage: 5);

        var (response, output) = await fixture.ApiClient
            .Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.Total.Should().Be(exampleCategoriesList.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().HaveCount(input.PerPage);
        foreach (var item in output.Data)
        {
            var expectedItem = exampleCategoriesList.Single(x => x.Id == item.Id);

            item.Should().NotBeNull();
            item.Id.Should().Be(expectedItem.Id);
            item.Name.Should().Be(expectedItem.Name);
            item.Description.Should().Be(expectedItem.Description);
            item.IsActive.Should().Be(expectedItem.IsActive);
            item.CreatedAt.TrimMilliseconds().Should().Be(expectedItem.CreatedAt.TrimMilliseconds());
        }
    }

    [Theory(DisplayName = nameof(ListPaginated))]
    [Trait("E2E/API", "Category/List - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPaginated(
        int quantityCategoriesToGenerate,
        int page,
        int perPage,
        int expectedTotal)
    {
        var exampleCategoriesList = fixture.GetExampleCategoriesList(quantityCategoriesToGenerate);
        await fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput(page, perPage, "", "", SearchOrder.Asc);

        var (response, output) = await fixture.ApiClient
            .Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.Total.Should().Be(exampleCategoriesList.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().HaveCount(expectedTotal);
        foreach (var item in output.Data)
        {
            var expectedItem = exampleCategoriesList.Single(x => x.Id == item.Id);

            item.Should().NotBeNull();
            item.Id.Should().Be(expectedItem.Id);
            item.Name.Should().Be(expectedItem.Name);
            item.Description.Should().Be(expectedItem.Description);
            item.IsActive.Should().Be(expectedItem.IsActive);
            item.CreatedAt.TrimMilliseconds().Should().Be(expectedItem.CreatedAt.TrimMilliseconds());
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("E2E/API", "Category/List - Endpoints")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(
        string search,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems)
    {
        var exampleCategoriesList = fixture.GetExampleCategoryListWithNames(
        [
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi AI",
            "Sci-fi Space",
            "Sci-fi Robots",
            "Sci-fi Future",
        ]);
        await fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput(page, perPage, search, "", SearchOrder.Asc);

        var (response, output) = await fixture.ApiClient
            .Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.Total.Should().Be(expectedQuantityTotalItems);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().HaveCount(expectedQuantityItemsReturned);
        foreach (var item in output.Data)
        {
            var expectedItem = exampleCategoriesList.Single(x => x.Id == item.Id);

            item.Should().NotBeNull();
            item.Id.Should().Be(expectedItem.Id);
            item.Name.Should().Be(expectedItem.Name);
            item.Description.Should().Be(expectedItem.Description);
            item.IsActive.Should().Be(expectedItem.IsActive);
            item.CreatedAt.TrimMilliseconds().Should().Be(expectedItem.CreatedAt.TrimMilliseconds());
        }
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("E2E/API", "Category/List - Endpoints")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("", "asc")]
    [InlineData("", "desc")]
    public async Task SearchOrdered(string orderBy, string order)
    {
        var exampleCategoriesList = fixture.GetExampleCategoriesList(10);
        await fixture.Persistence.InsertList(exampleCategoriesList);
        var searchOrder = order.Equals("asc", StringComparison.CurrentCultureIgnoreCase)
            ? SearchOrder.Asc
            : SearchOrder.Desc;
        var input = new ListCategoriesInput(page: 1, perPage: 20, search: "", orderBy, searchOrder);

        var (response, output) = await fixture.ApiClient
            .Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.Total.Should().Be(exampleCategoriesList.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().HaveCount(exampleCategoriesList.Count);
        var expectedOutput = fixture
            .CloneCategoriesListOrdered(exampleCategoriesList, orderBy, searchOrder);

        for (int index = 0; index < exampleCategoriesList.Count; index++)
        {
            var expectedItem = expectedOutput[index];
            var outputItem = output.Data[index];

            expectedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.TrimMilliseconds().Should().Be(expectedItem.CreatedAt.TrimMilliseconds());
        }
    }

    [Theory(DisplayName = nameof(SearchOrderedDates))]
    [Trait("E2E/API", "Category/List - Endpoints")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    public async Task SearchOrderedDates(string orderBy, string order)
    {
        var exampleCategoriesList = fixture.GetExampleCategoriesList(10);
        await fixture.Persistence.InsertList(exampleCategoriesList);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var input = new ListCategoriesInput(page: 1, perPage: 20, search: "", orderBy, searchOrder);

        var (response, output) = await fixture.ApiClient
            .Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.Total.Should().Be(exampleCategoriesList.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().HaveCount(exampleCategoriesList.Count);

        DateTime lastItemDate = default;
        foreach (var item in output.Data)
        {
            var expectedItem = exampleCategoriesList.Single(x => x.Id == item.Id);

            item.Should().NotBeNull();
            item.Id.Should().Be(expectedItem.Id);
            item.Name.Should().Be(expectedItem.Name);
            item.Description.Should().Be(expectedItem.Description);
            item.IsActive.Should().Be(expectedItem.IsActive);
            item.CreatedAt.TrimMilliseconds().Should().Be(expectedItem.CreatedAt.TrimMilliseconds());
            if (lastItemDate != default)
            {
                if (order == "asc")
                    Assert.True(item.CreatedAt >= lastItemDate);
                else
                    Assert.True(item.CreatedAt <= lastItemDate);
            }

            lastItemDate = item.CreatedAt;
        }
    }

    public void Dispose() => fixture.CleanPersistence();
}