using FC.Codeflix.Catalog.Application.UseCases.Genres.CreateGenre;
using FC.Codeflix.Catalog.Domain.Entities;

using FluentAssertions;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genres.CreateGenre;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest(CreateGenreTestFixture fixture)
{
    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("Application", "CreateGenre - Use Cases")]
    public async Task CreateGenre()
    {
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();
        var useCase = new CreateGenreUseCase(genreRepositoryMock.Object, unitOfWorkMock.Object);
        var createGenreInput = fixture.GetExampleInput();

        var output = await useCase.Handle(createGenreInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(createGenreInput.Name);
        output.IsActive.Should().Be(createGenreInput.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().BeEmpty();
        genreRepositoryMock
            .Verify(x => x.Insert(It.IsAny<Genre>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock
            .Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(CreateWithRelatedCategories))]
    [Trait("Application", "CreateGenre - Use Cases")]
    public async Task CreateWithRelatedCategories()
    {
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();
        var useCase = new CreateGenreUseCase(genreRepositoryMock.Object, unitOfWorkMock.Object);
        var input = fixture.GetExampleInputWithCategories();

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().HaveCount(input.CategoriesIds?.Count ?? 0);
        input.CategoriesIds?.ForEach(id => output.Categories.Should().Contain(id));
        genreRepositoryMock
            .Verify(x => x.Insert(It.IsAny<Genre>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock
            .Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}
