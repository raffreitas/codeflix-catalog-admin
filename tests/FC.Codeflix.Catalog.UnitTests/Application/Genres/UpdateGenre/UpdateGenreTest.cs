using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genres.UpdateGenre;
using FC.Codeflix.Catalog.Domain.Exceptions;

using FluentAssertions;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genres.UpdateGenre;

[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTest(UpdateGenreTestFixture fixture)
{
    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenre()
    {
        var exampleGenre = fixture.GetExampleGenre();
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();
        var newName = fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleGenre);

        var input = new UpdateGenreInput(
            exampleGenre.Id,
            newName,
            newIsActive
        );

        var useCase = new UpdateGenreUseCase(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            fixture.GetCategoryRepositoryMock().Object
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(newName);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Id.Should().Be(exampleGenre.Id);
        output.Categories.Should().HaveCount(0);

        genreRepositoryMock.Verify(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        var exampleId = Guid.NewGuid();

        genreRepositoryMock.Setup(x => x.Get(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>())
        ).ThrowsAsync(new NotFoundException($"Genre '{exampleId}' not found."));

        var input = new UpdateGenreInput(
            exampleId,
            fixture.GetValidGenreName(),
            true
        );

        var useCase = new UpdateGenreUseCase(
            genreRepositoryMock.Object,
            fixture.GetUnitOfWorkMock().Object,
            fixture.GetCategoryRepositoryMock().Object
        );

        var act = async () => await useCase.Handle(input, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{exampleId}' not found.");

    }

    [Theory(DisplayName = nameof(ThrowWhenNameIsInvalid))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task ThrowWhenNameIsInvalid(string? name)
    {
        var exampleGenre = fixture.GetExampleGenre();
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleGenre);

        var input = new UpdateGenreInput(
            exampleGenre.Id,
            name!,
            newIsActive
        );

        var useCase = new UpdateGenreUseCase(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            fixture.GetCategoryRepositoryMock().Object
        );

        var act = async () => await useCase.Handle(input, CancellationToken.None);

        await act.Should().ThrowAsync<EntityValidationException>()
            .WithMessage($"Name should not be null or empty.");
    }
}
