using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genres.DeleteGenre;
using FC.Codeflix.Catalog.Domain.Entities;

using FluentAssertions;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genres.DeleteGenre;

[Collection(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTest(DeleteGenreTestFixture fixture)
{
    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenre()
    {
        var exampleGenre = fixture.GetExampleGenre();
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleGenre);

        var input = new DeleteGenreInput(
            exampleGenre.Id
        );

        var useCase = new DeleteGenreUseCase(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(x => x.Delete(
            It.Is<Genre>(x => x.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "DeleteGenre - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var exampleId = Guid.NewGuid();
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleId),
            It.IsAny<CancellationToken>())
        ).ThrowsAsync(new NotFoundException($"Genre '{exampleId} not found.'"));

        var input = new DeleteGenreInput(exampleId);

        var useCase = new DeleteGenreUseCase(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var act = async () => await useCase.Handle(input, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{exampleId} not found.'");

        genreRepositoryMock.Verify(x => x.Get(
            It.Is<Guid>(x => x == exampleId),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
