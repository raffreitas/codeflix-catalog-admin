using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genres.CreateGenre;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Exceptions;

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
        var categoryRepositoryMock = fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();
        var useCase = new CreateGenreUseCase(genreRepositoryMock.Object, categoryRepositoryMock.Object,
            unitOfWorkMock.Object);
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
        var input = fixture.GetExampleInputWithCategories();
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = fixture.GetCategoryRepositoryMock();
        categoryRepositoryMock
            .Setup(x => x
                .GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
            ).ReturnsAsync(input.CategoriesIds!);
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();
        var useCase = new CreateGenreUseCase(genreRepositoryMock.Object, categoryRepositoryMock.Object,
            unitOfWorkMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().HaveCount(input.CategoriesIds?.Count ?? 0);
        input.CategoriesIds?.ForEach(id => output.Categories.Should().Contain(r => r.Id == id));
        genreRepositoryMock
            .Verify(x => x.Insert(It.IsAny<Genre>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock
            .Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(CreateThrowWhenRelatedCategoryNotFound))]
    [Trait("Application", "CreateGenre - Use Cases")]
    public async Task CreateThrowWhenRelatedCategoryNotFound()
    {
        var input = fixture.GetExampleInputWithCategories();
        var exampleGuid = input.CategoriesIds![^1];
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(input.CategoriesIds.FindAll(x => x != exampleGuid).AsReadOnly());

        var useCase = new CreateGenreUseCase(
            genreRepositoryMock.Object,
            categoryRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var act = async () => await useCase.Handle(input, CancellationToken.None);

        await act.Should()
            .ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {exampleGuid}");

        categoryRepositoryMock
            .Verify(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory(DisplayName = nameof(ThrowWhenNameIsInvalid))]
    [Trait("Application", "CreateGenre - Use Cases")]
    [InlineData("")]
    [InlineData("  ")]
    public async Task ThrowWhenNameIsInvalid(string name)
    {
        var input = fixture.GetExampleInput(name);
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();

        var useCase = new CreateGenreUseCase(
            genreRepositoryMock.Object,
            categoryRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var act = async () => await useCase.Handle(input, CancellationToken.None);

        await act.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage($"Name should not be null or empty.");
    }

    [Fact(DisplayName = nameof(ThrowWhenNameIsNull))]
    [Trait("Application", "CreateGenre - Use Cases")]
    public async Task ThrowWhenNameIsNull()
    {
        var input = fixture.GetExampleInput(null!);
        var genreRepositoryMock = fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();

        var useCase = new CreateGenreUseCase(
            genreRepositoryMock.Object,
            categoryRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var act = async () => await useCase.Handle(input, CancellationToken.None);

        await act.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage($"Name should not be null or empty.");
    }
}