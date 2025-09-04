using FC.Codeflix.Catalog.Application.UseCases.Categories.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

using FluentAssertions;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Categories.ListCategories;

[Collection(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTest(ListCategoriesTestFixture fixture)
{
    [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    public async Task SearchReturnsListAndTotal()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleCategoryList = fixture.GetExampleCategoryList();
        await dbContext.AddRangeAsync(exampleCategoryList, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var input = new ListCategoriesInput(1, 20, "", "", SearchOrder.Asc);
        var useCase = new ListCategoriesUseCase(categoryRepository);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleCategoryList.Count);
        output.Items.Should().HaveCount(exampleCategoryList.Count);

        foreach (var outputItem in output.Items)
        {
            var exampleItem = exampleCategoryList
                .Single(x => x.Id == outputItem.Id);

            outputItem.Should().NotBeNull();
            outputItem.Id.Should().Be(exampleItem.Id);
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    public async Task SearchReturnsEmptyWhenPersistenceIsEmpty()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var categoryRepository = new CategoryRepository(dbContext);
        var input = new ListCategoriesInput(1, 20, "", "", SearchOrder.Asc);
        var useCase = new ListCategoriesUseCase(categoryRepository);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }

    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPaginated(
        int quantityCategoriesToGenerate,
        int page,
        int perPage,
        int expectedTotal)
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleCategoryList = fixture.GetExampleCategoryList(quantityCategoriesToGenerate);
        await dbContext.AddRangeAsync(exampleCategoryList, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var input = new ListCategoriesInput(page, perPage, "", "", SearchOrder.Asc);
        var useCase = new ListCategoriesUseCase(categoryRepository);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(quantityCategoriesToGenerate);
        output.Items.Should().HaveCount(expectedTotal);

        foreach (var outputItem in output.Items)
        {
            var exampleItem = exampleCategoryList
                .Single(x => x.Id == outputItem.Id);

            outputItem.Should().NotBeNull();
            outputItem.Id.Should().Be(exampleItem.Id);
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
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
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleCategoryList = fixture.GetExampleCategoryListWithNames(
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
        await dbContext.AddRangeAsync(exampleCategoryList, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var input = new ListCategoriesInput(page, perPage, search, "", SearchOrder.Asc);
        var useCase = new ListCategoriesUseCase(categoryRepository);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);

        foreach (var outputItem in output.Items)
        {
            var exampleItem = exampleCategoryList
                .Single(x => x.Id == outputItem.Id);

            outputItem.Should().NotBeNull();
            outputItem.Id.Should().Be(exampleItem.Id);
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    [InlineData("", "desc")]
    public async Task SearchOrdered(
        string orderBy,
        string order
    )
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleCategoryList = fixture.GetExampleCategoryList(10);
        await dbContext.AddRangeAsync(exampleCategoryList, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var input = new ListCategoriesInput(1, 20, string.Empty, orderBy, searchOrder);
        var useCase = new ListCategoriesUseCase(categoryRepository);

        var output = await useCase.Handle(input, CancellationToken.None);

        var expectedOutput = fixture
            .CloneCategoriesListOrdered(exampleCategoryList, orderBy, searchOrder);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleCategoryList.Count);
        output.Items.Should().HaveCount(exampleCategoryList.Count);

        for (int index = 0; index < exampleCategoryList.Count; index++)
        {
            var expectedItem = expectedOutput[index];
            var outputItem = output.Items[index];

            expectedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }
}
