using FC.Codeflix.Catalog.Application.UseCases.Categories.ListCategories;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

using FluentAssertions;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.ListCategories;

[Collection(nameof(ListCategoriesTestFixtureCollection))]
public class ListCategoriesTest
{
    private readonly ListCategoriesTestFixture _fixture;
    public ListCategoriesTest(ListCategoriesTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(List))]
    [Trait("Application", "ListCategories - Use Cases")]
    public async Task List()
    {
        var repositoryMock = _fixture.GetCategoryRepositoryMock();
        var categoriesExampleList = _fixture.GetExampleCategoriesList();
        var input = _fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<Category>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: categoriesExampleList,
            total: new Random().Next(50, 200)
        );
        repositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page &&
                searchInput.PerPage == input.PerPage &&
                searchInput.Search == input.Search &&
                searchInput.OrderBy == input.Sort &&
                searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(outputRepositorySearch);
        var useCase = new ListCategoriesUseCase(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var repositoryCategory = outputRepositorySearch.Items
                .First(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(repositoryCategory.Name);
            outputItem.Description.Should().Be(repositoryCategory.Description);
            outputItem.IsActive.Should().Be(repositoryCategory.IsActive);
            outputItem.CreatedAt.Should().Be(repositoryCategory.CreatedAt);
        });
        repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page &&
                searchInput.PerPage == input.PerPage &&
                searchInput.Search == input.Search &&
                searchInput.OrderBy == input.Sort &&
                searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Theory(DisplayName = nameof(ListInputWithoutAllParameters))]
    [Trait("Application", "ListCategories - Use Cases")]
    [MemberData(
        nameof(ListCategoriesTestDataGenerator.GetInputsWithoutAllParameters),
        parameters: 14,
        MemberType = typeof(ListCategoriesTestDataGenerator))]
    public async Task ListInputWithoutAllParameters(ListCategoriesInput input)
    {
        var repositoryMock = _fixture.GetCategoryRepositoryMock();
        var categoriesExampleList = _fixture.GetExampleCategoriesList();
        var outputRepositorySearch = new SearchOutput<Category>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: categoriesExampleList,
            total: new Random().Next(50, 200)
        );
        repositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page &&
                searchInput.PerPage == input.PerPage &&
                searchInput.Search == input.Search &&
                searchInput.OrderBy == input.Sort &&
                searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(outputRepositorySearch);
        var useCase = new ListCategoriesUseCase(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var repositoryCategory = outputRepositorySearch.Items
                .First(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(repositoryCategory.Name);
            outputItem.Description.Should().Be(repositoryCategory.Description);
            outputItem.IsActive.Should().Be(repositoryCategory.IsActive);
            outputItem.CreatedAt.Should().Be(repositoryCategory.CreatedAt);
        });
        repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page &&
                searchInput.PerPage == input.PerPage &&
                searchInput.Search == input.Search &&
                searchInput.OrderBy == input.Sort &&
                searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact(DisplayName = nameof(ListOkWhenEmpty))]
    [Trait("Application", "ListCategories - Use Cases")]
    public async Task ListOkWhenEmpty()
    {
        var repositoryMock = _fixture.GetCategoryRepositoryMock();
        var categoriesExampleList = _fixture.GetExampleCategoriesList();
        var input = _fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<Category>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: [],
            total: 0
        );
        repositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page &&
                searchInput.PerPage == input.PerPage &&
                searchInput.Search == input.Search &&
                searchInput.OrderBy == input.Sort &&
                searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(outputRepositorySearch);
        var useCase = new ListCategoriesUseCase(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
        repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page &&
                searchInput.PerPage == input.PerPage &&
                searchInput.Search == input.Search &&
                searchInput.OrderBy == input.Sort &&
                searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}
