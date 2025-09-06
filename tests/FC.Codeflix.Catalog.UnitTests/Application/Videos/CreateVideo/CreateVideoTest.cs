using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Videos.CreateVideo;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.UnitTests.Domain.Entities.Videos;

using FluentAssertions;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Videos.CreateVideo;

[Trait("Application", "CreateVideo - Use Cases")]
public sealed class CreateVideoTest(VideoTestFixture fixture) : IClassFixture<VideoTestFixture>
{
    [Fact(DisplayName = nameof(CreateVideo))]
    public async Task CreateVideo()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new CreateVideoUseCase(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object
        );
        var input = fixture.CreateValidCreateVideoInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.Insert(
            It.Is<Video>(video =>
                video.Title == input.Title &&
                video.Published == input.Published &&
                video.Description == input.Description &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating &&
                video.Id != Guid.Empty &&
                video.YearLaunched == input.YearLaunched &&
                video.Opened == input.Opened
            ),
            It.IsAny<CancellationToken>())
        );
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
    }

    [Fact(DisplayName = nameof(CreateVideoWithCategoriesIds))]
    public async Task CreateVideoWithCategoriesIds()
    {
        var exampleCategoriesIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleCategoriesIds);
        var useCase = new CreateVideoUseCase(
            videoRepositoryMock.Object,
            categoryRepositoryMock.Object,
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object
        );
        var input = fixture.CreateValidCreateVideoInput(exampleCategoriesIds);

        var output = await useCase.Handle(input, CancellationToken.None);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.CategoriesIds.Should().BeEquivalentTo(exampleCategoriesIds);
        videoRepositoryMock.Verify(x => x.Insert(
            It.Is<Video>(video =>
                video.Title == input.Title &&
                video.Published == input.Published &&
                video.Description == input.Description &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating &&
                video.Id != Guid.Empty &&
                video.YearLaunched == input.YearLaunched &&
                video.Opened == input.Opened &&
                video.Categories.All(categoryId => exampleCategoriesIds.Contains(categoryId))
            ),
            It.IsAny<CancellationToken>())
        );
        categoryRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ThrowsWhenCategoryIdInvalid))]
    public async Task ThrowsWhenCategoryIdInvalid()
    {
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var exampleCategoriesIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var removedCategoryId = exampleCategoriesIds[2];
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleCategoriesIds.FindAll(x => x != removedCategoryId).AsReadOnly());
        var useCase = new CreateVideoUseCase(
            videoRepositoryMock.Object,
            categoryRepositoryMock.Object,
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object
        );
        var input = fixture.CreateValidCreateVideoInput(exampleCategoriesIds);

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {removedCategoryId}.");
        categoryRepositoryMock.VerifyAll();
    }

    [Theory(DisplayName = nameof(CreateVideoThrowsWithInvalidInput))]
    [ClassData(typeof(CreateVideoTestDataGenerator))]
    public async Task CreateVideoThrowsWithInvalidInput(
        CreateVideoInput input,
        string expectedValidationError)
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new CreateVideoUseCase(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object
        );

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        var exceptionAssertion = await action.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage($"There are validation errors");
        exceptionAssertion.Which.Errors!.ToList()[0].Message.Should()
            .Be(expectedValidationError);
        repositoryMock.Verify(
            x => x.Insert(It.IsAny<Video>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact(DisplayName = nameof(CreateVideoWithGenresIds))]
    public async Task CreateVideoWithGenresIds()
    {
        var exampleIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        genreRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleIds);
        var useCase = new CreateVideoUseCase(
            videoRepositoryMock.Object,
            categoryRepositoryMock.Object,
            genreRepositoryMock.Object,
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object
        );
        var input = fixture.CreateValidCreateVideoInput(genresIds: exampleIds);

        var output = await useCase.Handle(input, CancellationToken.None);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.CategoriesIds.Should().BeEmpty();
        output.GenresIds.Should().BeEquivalentTo(exampleIds);
        videoRepositoryMock.Verify(x => x.Insert(
            It.Is<Video>(video =>
                video.Title == input.Title &&
                video.Published == input.Published &&
                video.Description == input.Description &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating &&
                video.Id != Guid.Empty &&
                video.YearLaunched == input.YearLaunched &&
                video.Opened == input.Opened &&
                video.Genres.All(id => exampleIds.Contains(id))
            ),
            It.IsAny<CancellationToken>())
        );
        genreRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ThrowsWhenInvalidGenreId))]
    public async Task ThrowsWhenInvalidGenreId()
    {
        var exampleIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var removedId = exampleIds[2];
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        genreRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleIds.FindAll(id => id != removedId));
        var useCase = new CreateVideoUseCase(
            videoRepositoryMock.Object,
            categoryRepositoryMock.Object,
            genreRepositoryMock.Object,
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object
        );
        var input = fixture.CreateValidCreateVideoInput(genresIds: exampleIds);

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related genre id (or ids) not found: {removedId}.");
        genreRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(CreateVideoWithCastMembersIds))]
    public async Task CreateVideoWithCastMembersIds()
    {
        var exampleIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleIds);
        var useCase = new CreateVideoUseCase(
            videoRepositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            castMemberRepositoryMock.Object,
            unitOfWorkMock.Object
        );
        var input = fixture.CreateValidCreateVideoInput(castMembersIds: exampleIds);

        var output = await useCase.Handle(input, CancellationToken.None);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.CategoriesIds.Should().BeEmpty();
        output.GenresIds.Should().BeEmpty();
        output.CastMembersIds.Should().BeEquivalentTo(exampleIds);
        videoRepositoryMock.Verify(x => x.Insert(
            It.Is<Video>(video =>
                video.Title == input.Title &&
                video.Published == input.Published &&
                video.Description == input.Description &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating &&
                video.Id != Guid.Empty &&
                video.YearLaunched == input.YearLaunched &&
                video.Opened == input.Opened &&
                video.CastMembers.All(id => exampleIds.Contains(id))
            ),
            It.IsAny<CancellationToken>())
        );
        castMemberRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ThrowsWhenInvalidCastMemberId))]
    public async Task ThrowsWhenInvalidCastMemberId()
    {
        var exampleIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var removedId = exampleIds[2];
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleIds.FindAll(x => x != removedId));
        var useCase = new CreateVideoUseCase(
            videoRepositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            castMemberRepositoryMock.Object,
            unitOfWorkMock.Object
        );
        var input = fixture.CreateValidCreateVideoInput(castMembersIds: exampleIds);

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related cast member id (or ids) not found: {removedId}.");
        castMemberRepositoryMock.VerifyAll();
    }
}