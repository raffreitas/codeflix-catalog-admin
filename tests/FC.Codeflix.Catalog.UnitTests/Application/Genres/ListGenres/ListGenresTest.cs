using FC.Codeflix.Catalog.Application.UseCases.Genres.ListGenres;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

using FluentAssertions;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genres.ListGenres;

[Collection(nameof(ListGenresTestFixture))]
public class ListGenresTest(ListGenresTestFixture fixture)
{
    [Fact(DisplayName = nameof(ListGenres))]
    [Trait("Application", "ListGenres - Use Cases")]
    public async Task ListGenres()
    {
        var repositoryMock = fixture.GetGenreRepositoryMock();
        var genresExampleList = fixture.GetExampleGenresList();
        var categoryRepositoryMock = fixture.GetCategoryRepositoryMock();

        var input = fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<Genre>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: genresExampleList,
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
        var useCase = new ListGenresUseCase(repositoryMock.Object, categoryRepositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var repositoryGenre = outputRepositorySearch.Items
                .First(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(repositoryGenre.Name);
            outputItem.IsActive.Should().Be(repositoryGenre.IsActive);
            outputItem.CreatedAt.Should().Be(repositoryGenre.CreatedAt);
            outputItem.Categories.Should().HaveCount(repositoryGenre.Categories.Count);
            foreach (var expectedId in repositoryGenre.Categories)
                outputItem.Categories.Should().Contain(relation => relation.Id == expectedId);
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

    [Fact(DisplayName = nameof(ListEmpty))]
    [Trait("Application", "ListGenres - Use Cases")]
    public async Task ListEmpty()
    {
        var repositoryMock = fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = fixture.GetCategoryRepositoryMock();
        var input = fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<Genre>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: [],
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
        var useCase = new ListGenresUseCase(repositoryMock.Object, categoryRepositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var repositoryGenre = outputRepositorySearch.Items
                .First(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(repositoryGenre.Name);
            outputItem.IsActive.Should().Be(repositoryGenre.IsActive);
            outputItem.CreatedAt.Should().Be(repositoryGenre.CreatedAt);
            outputItem.Categories.Should().HaveCount(repositoryGenre.Categories.Count);
            foreach (var expectedId in repositoryGenre.Categories)
                outputItem.Categories.Should().Contain(x => x.Id == expectedId);
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

    [Fact(DisplayName = nameof(ListUsingDefaultValues))]
    [Trait("Application", "ListGenres - Use Cases")]
    public async Task ListUsingDefaultValues()
    {
        var repositoryMock = fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = fixture.GetCategoryRepositoryMock();
        var outputRepositorySearch = new SearchOutput<Genre>(
            currentPage: 1,
            perPage: 15,
            items: [],
            total: 0
        );
        repositoryMock.Setup(x => x.Search(
            It.IsAny<SearchInput>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(outputRepositorySearch);
        var useCase = new ListGenresUseCase(repositoryMock.Object, categoryRepositoryMock.Object);

        await useCase.Handle(new ListGenresInput(), CancellationToken.None);

        repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == 1 &&
                searchInput.PerPage == 15 &&
                searchInput.Search == string.Empty &&
                searchInput.OrderBy == string.Empty &&
                searchInput.Order == SearchOrder.Asc
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}