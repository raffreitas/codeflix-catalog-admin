using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genres.GetGenre;

using FluentAssertions;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genres.GetGenre;

[Collection(nameof(GetGenreTestFixture))]
public class GetGenreTest(GetGenreTestFixture fixture)
{
    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("Application", "GetGenre - Use Cases")]
    public async Task GetGenre()
    {
        var exampleGenre = fixture.GetExampleGenre(categoriesIds: fixture.GetRandomIdsList());
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleGenre);

        var input = new GetGenreInput(
            exampleGenre.Id
        );

        var useCase = new GetGenreUseCase(
            genreRepositoryMock.Object
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(exampleGenre.Name);
        output.IsActive.Should().Be(exampleGenre.IsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Id.Should().Be(exampleGenre.Id);
        output.Categories.Should().HaveCount(exampleGenre.Categories.Count);
        foreach (var expectedId in exampleGenre.Categories)
            output.Categories.Should().Contain(expectedId);

        genreRepositoryMock.Verify(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "GetGenre - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var exampleId = Guid.NewGuid();
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleId),
            It.IsAny<CancellationToken>())
        ).ThrowsAsync(new NotFoundException($"Genre '{exampleId} not found.'"));

        var input = new GetGenreInput(exampleId);

        var useCase = new GetGenreUseCase(genreRepositoryMock.Object);

        var act = async () => await useCase.Handle(input, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{exampleId} not found.'");

        genreRepositoryMock.Verify(x => x.Get(
            It.Is<Guid>(x => x == exampleId),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
