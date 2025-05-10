using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genres.UpdateGenre;
using FC.Codeflix.Catalog.Domain.Entities;
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

        genreRepositoryMock.Verify(x => x.Update(
            It.Is<Genre>(x => x.Id == exampleGenre.Id),
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

    [Theory(DisplayName = nameof(UpdateGenreOnlyName))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateGenreOnlyName(bool isActive)
    {
        var exampleGenre = fixture.GetExampleGenre(isActive: isActive);
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();
        var newName = fixture.GetValidGenreName();
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleGenre);

        var input = new UpdateGenreInput(
            exampleGenre.Id,
            newName
        );

        var useCase = new UpdateGenreUseCase(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            fixture.GetCategoryRepositoryMock().Object
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(newName);
        output.IsActive.Should().Be(exampleGenre.IsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Id.Should().Be(exampleGenre.Id);
        output.Categories.Should().HaveCount(0);

        genreRepositoryMock.Verify(x => x.Update(
            It.Is<Genre>(x => x.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateGenreAddingCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreAddingCategoriesIds()
    {
        var exampleGenre = fixture.GetExampleGenre();
        var exampleCategoriesIds = fixture.GetRandomIdsList();
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();
        var newName = fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleGenre);
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleCategoriesIds);

        var input = new UpdateGenreInput(
            exampleGenre.Id,
            newName,
            newIsActive,
            exampleCategoriesIds
        );

        var useCase = new UpdateGenreUseCase(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(newName);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Id.Should().Be(exampleGenre.Id);
        output.Categories.Should().HaveCount(exampleCategoriesIds.Count);
        exampleCategoriesIds.ForEach(expectedId
            => output.Categories.Should().Contain(expectedId)
        );

        genreRepositoryMock.Verify(x => x.Update(
            It.Is<Genre>(x => x.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateGenreReplacingCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreReplacingCategoriesIds()
    {
        var exampleGenre = fixture.GetExampleGenre(categoriesIds: fixture.GetRandomIdsList());
        var exampleCategoriesIds = fixture.GetRandomIdsList();
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();
        var newName = fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleGenre);
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleCategoriesIds);

        var input = new UpdateGenreInput(
            exampleGenre.Id,
            newName,
            newIsActive,
            exampleCategoriesIds
        );

        var useCase = new UpdateGenreUseCase(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(newName);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Id.Should().Be(exampleGenre.Id);
        output.Categories.Should().HaveCount(exampleCategoriesIds.Count);
        exampleCategoriesIds.ForEach(expectedId
            => output.Categories.Should().Contain(expectedId)
        );

        genreRepositoryMock.Verify(x => x.Update(
            It.Is<Genre>(x => x.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowWenCategoryNotFound))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task ThrowWenCategoryNotFound()
    {
        var exampleGenre = fixture.GetExampleGenre(categoriesIds: fixture.GetRandomIdsList());
        var exampleNewCategoriesIds = fixture.GetRandomIdsList(10);
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();
        var listReturnedByCategoryRepository = exampleNewCategoriesIds
            .GetRange(0, exampleNewCategoriesIds.Count - 2);
        var idsNotReturnedByCategoryRepository = exampleNewCategoriesIds
            .GetRange(exampleNewCategoriesIds.Count - 2, 2);
        var newName = fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(listReturnedByCategoryRepository);
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleGenre);


        var input = new UpdateGenreInput(
            exampleGenre.Id,
            newName,
            newIsActive,
            exampleNewCategoriesIds
        );

        var useCase = new UpdateGenreUseCase(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
        );

        var act = async () => await useCase.Handle(input, CancellationToken.None);

        var notFoundCategories = string.Join(", ", idsNotReturnedByCategoryRepository);

        await act.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id or ids not found: '{notFoundCategories}'");
    }

    [Fact(DisplayName = nameof(UpdateGenreWithoutCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithoutCategoriesIds()
    {
        var exampleCategoriesIds = fixture.GetRandomIdsList();
        var exampleGenre = fixture.GetExampleGenre(categoriesIds: exampleCategoriesIds);
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = fixture.GetCategoryRepositoryMock();
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
            categoryRepositoryMock.Object
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(newName);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Id.Should().Be(exampleGenre.Id);
        output.Categories.Should().HaveCount(exampleCategoriesIds.Count);
        exampleCategoriesIds.ForEach(expectedId
            => output.Categories.Should().Contain(expectedId)
        );

        genreRepositoryMock.Verify(x => x.Update(
            It.Is<Genre>(x => x.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateGenreWithEmptyCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithEmptyCategoriesIds()
    {
        var exampleCategoriesIds = fixture.GetRandomIdsList();
        var exampleGenre = fixture.GetExampleGenre(categoriesIds: exampleCategoriesIds);
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = fixture.GetCategoryRepositoryMock();
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
            newIsActive,
            []
        );

        var useCase = new UpdateGenreUseCase(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(newName);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Id.Should().Be(exampleGenre.Id);
        output.Categories.Should().HaveCount(0);

        genreRepositoryMock.Verify(x => x.Update(
            It.Is<Genre>(x => x.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}
