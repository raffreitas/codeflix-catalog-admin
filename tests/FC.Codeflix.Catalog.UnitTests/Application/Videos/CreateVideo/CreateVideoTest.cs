using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Videos.CreateVideo;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.UnitTests.Domain.Entities.Videos;

using FluentAssertions;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Videos.CreateVideo;

[Trait("Application", "CreateVideo - Use Cases")]
public sealed class CreateVideoTest : IClassFixture<VideoTestFixture>
{
    private readonly Mock<IVideoRepository> _videoRepositoryMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
    private readonly Mock<IGenreRepository> _genreRepositoryMock = new();
    private readonly Mock<ICastMemberRepository> _castMemberRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IStorageService> _storageServiceMock = new();

    private readonly VideoTestFixture _fixture;
    private readonly CreateVideoUseCase _useCase;

    public CreateVideoTest(VideoTestFixture fixture)
    {
        _fixture = fixture;
        _useCase = new CreateVideoUseCase(
            _videoRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _genreRepositoryMock.Object,
            _castMemberRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _storageServiceMock.Object
        );
    }


    [Fact(DisplayName = nameof(CreateVideo))]
    public async Task CreateVideo()
    {
        var input = _fixture.CreateValidCreateVideoInput();

        var output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.Verify(x => x.Insert(
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
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
    }

    [Fact(DisplayName = nameof(CreateVideoWithAllImages))]
    public async Task CreateVideoWithAllImages()
    {
        const string expectedThumbName = "thumb.jpg";
        const string expectedThumbHalfName = "thumb-half.jpg";
        const string expectedBannerName = "banner.jpg";
        _storageServiceMock.Setup(x => x
            .Upload(
                It.Is<string>(fileName => fileName.EndsWith("-banner.jpg")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
        ).ReturnsAsync(expectedBannerName);
        _storageServiceMock.Setup(x => x
            .Upload(
                It.Is<string>(fileName => fileName.EndsWith("-thumbhalf.jpg")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
        ).ReturnsAsync(expectedThumbHalfName);
        _storageServiceMock.Setup(x => x
            .Upload(
                It.Is<string>(fileName => fileName.EndsWith("-thumb.jpg")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
        ).ReturnsAsync(expectedThumbName);

        var input = _fixture.CreateValidCreateVideoInputWithAllImages();

        var output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.Verify(x => x.Insert(
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
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbFileUrl.Should().Be(expectedThumbName);
        output.ThumbHalfFileUrl.Should().Be(expectedThumbHalfName);
        output.BannerFileUrl.Should().Be(expectedBannerName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithThumb))]
    public async Task CreateVideoWithThumb()
    {
        const string expectedThumbName = "thumb.jpg";
        _storageServiceMock.Setup(x => x
                .Upload(
                    It.IsAny<string>(),
                    It.IsAny<Stream>(),
                    It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(expectedThumbName);

        var input = _fixture.CreateValidCreateVideoInput(thumb: _fixture.GetValidImageFileInput());

        var output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.Verify(x => x.Insert(
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
        _storageServiceMock.VerifyAll();
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbFileUrl.Should().Be(expectedThumbName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithThumbHalf))]
    public async Task CreateVideoWithThumbHalf()
    {
        const string expectedThumbHalfName = "thumb-half.jpg";
        _storageServiceMock.Setup(x => x
                .Upload(
                    It.IsAny<string>(),
                    It.IsAny<Stream>(),
                    It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(expectedThumbHalfName);

        var input = _fixture.CreateValidCreateVideoInput(thumbHalf: _fixture.GetValidImageFileInput());

        var output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.Verify(x => x.Insert(
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
        _storageServiceMock.VerifyAll();
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbHalfFileUrl.Should().Be(expectedThumbHalfName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithBanner))]
    public async Task CreateVideoWithBanner()
    {
        const string expectedBannerName = "banner.jpg";
        _storageServiceMock.Setup(x => x
                .Upload(
                    It.IsAny<string>(),
                    It.IsAny<Stream>(),
                    It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(expectedBannerName);

        var input = _fixture.CreateValidCreateVideoInput(banner: _fixture.GetValidImageFileInput());

        var output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.Verify(x => x.Insert(
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
        _storageServiceMock.VerifyAll();
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.BannerFileUrl.Should().Be(expectedBannerName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithMedia))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithMedia()
    {
        var expectedMediaName = $"/storage/{_fixture.GetValidMediaPath()}";
        _storageServiceMock.Setup(x => x.Upload(
            It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(expectedMediaName);
        var input = _fixture.CreateValidCreateVideoInput(media: _fixture.GetValidMediaFileInput());

        var output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.Verify(x => x.Insert(
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
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        _storageServiceMock.VerifyAll();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.VideoFileUrl.Should().Be(expectedMediaName);
    }


    [Fact(DisplayName = nameof(CreateVideoWithTrailer))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithTrailer()
    {
        var expectedTrailerName = $"/storage/{_fixture.GetValidMediaPath()}";
        _storageServiceMock.Setup(x => x.Upload(
            It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(expectedTrailerName);

        var input = _fixture.CreateValidCreateVideoInput(
            trailer: _fixture.GetValidMediaFileInput());

        var output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.Verify(x => x.Insert(
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
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        _storageServiceMock.VerifyAll();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.TrailerFileUrl.Should().Be(expectedTrailerName);
    }

    [Fact(DisplayName = nameof(ThrowsExceptionAndRollbackMediaUploadInCommitErrorCases))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsExceptionAndRollbackMediaUploadInCommitErrorCases()
    {
        var input = _fixture.CreateValidInputWithAllMedias();
        var storageMediaPath = _fixture.GetValidMediaPath();
        var storageTrailerPath = _fixture.GetValidMediaPath();
        var storagePathList = new List<string> { storageMediaPath, storageTrailerPath };
        _storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(fileName => fileName.EndsWith($"media.{input.Media!.Extension}")), It.IsAny<Stream>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(storageMediaPath);
        _storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(fileName => fileName.EndsWith($"trailer.{input.Trailer!.Extension}")), It.IsAny<Stream>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(storageTrailerPath);
        _unitOfWorkMock.Setup(x => x.Commit(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong with the commit"));

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Something went wrong with the commit");
        _storageServiceMock.Verify(
            x => x.Delete(
                It.Is<string>(path => storagePathList.Contains(path)),
                It.IsAny<CancellationToken>()
            ), Times.Exactly(2));
        _storageServiceMock.Verify(
            x => x.Delete(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ), Times.Exactly(2));
    }

    [Fact(DisplayName = nameof(ThrowsExceptionInUploadErrorCases))]
    public async Task ThrowsExceptionInUploadErrorCases()
    {
        _storageServiceMock.Setup(x => x
            .Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
        ).ThrowsAsync(new Exception("Something went wrong in upload"));
        var input = _fixture.CreateValidCreateVideoInputWithAllImages();

        var action = async () => await _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Something went wrong in upload");
    }

    [Fact(DisplayName = nameof(ThrowsExceptionAndRollbackUploadInImageUploadErrorCases))]
    public async Task ThrowsExceptionAndRollbackUploadInImageUploadErrorCases()
    {
        _storageServiceMock.Setup(x => x
            .Upload(
                It.Is<string>(fileName => fileName.EndsWith("-thumb.jpg")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
        ).ReturnsAsync("123-thumb.jpg");
        _storageServiceMock.Setup(x => x
            .Upload(
                It.Is<string>(fileName => fileName.EndsWith("-banner.jpg")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
        ).ReturnsAsync("123-banner.jpg");
        _storageServiceMock.Setup(x => x
            .Upload(
                It.Is<string>(fileName => fileName.EndsWith("-thumbhalf.jpg")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
        ).ThrowsAsync(new Exception("Something went wrong in upload"));
        var input = _fixture.CreateValidCreateVideoInputWithAllImages();

        var action = async () => await _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Something went wrong in upload");
        _storageServiceMock.Verify(x => x
                .Delete(It.Is<string>(fileName => (fileName == "123-thumb.jpg") || (fileName == "123-banner.jpg")),
                    It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Fact(DisplayName = nameof(CreateVideoWithCategoriesIds))]
    public async Task CreateVideoWithCategoriesIds()
    {
        var exampleCategoriesIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleCategoriesIds);
        var input = _fixture.CreateValidCreateVideoInput(exampleCategoriesIds);

        var output = await _useCase.Handle(input, CancellationToken.None);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        var outputItemCategoryIds = output.Categories.Select(categoryDto => categoryDto.Id);
        outputItemCategoryIds.Should().BeEquivalentTo(exampleCategoriesIds);
        _videoRepositoryMock.Verify(x => x.Insert(
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
        _categoryRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ThrowsWhenCategoryIdInvalid))]
    public async Task ThrowsWhenCategoryIdInvalid()
    {
        var exampleCategoriesIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var removedCategoryId = exampleCategoriesIds[2];
        _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleCategoriesIds.FindAll(x => x != removedCategoryId).AsReadOnly());
        var input = _fixture.CreateValidCreateVideoInput(exampleCategoriesIds);

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {removedCategoryId}.");
        _categoryRepositoryMock.VerifyAll();
    }

    [Theory(DisplayName = nameof(CreateVideoThrowsWithInvalidInput))]
    [ClassData(typeof(CreateVideoTestDataGenerator))]
    public async Task CreateVideoThrowsWithInvalidInput(
        CreateVideoInput input,
        string expectedValidationError)
    {
        var action = async () => await _useCase.Handle(input, CancellationToken.None);

        var exceptionAssertion = await action.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage("There are validation errors");
        exceptionAssertion.Which.Errors!.ToList()[0].Message.Should()
            .Be(expectedValidationError);
        _videoRepositoryMock.Verify(
            x => x.Insert(It.IsAny<Video>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact(DisplayName = nameof(CreateVideoWithGenresIds))]
    public async Task CreateVideoWithGenresIds()
    {
        var exampleIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        _genreRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleIds);
        var input = _fixture.CreateValidCreateVideoInput(genresIds: exampleIds);

        var output = await _useCase.Handle(input, CancellationToken.None);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Categories.Should().BeEmpty();
        var outputItemGenresIds = output.Genres.Select(dto => dto.Id).ToList();
        outputItemGenresIds.Should().BeEquivalentTo(exampleIds);
        _videoRepositoryMock.Verify(x => x.Insert(
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
        _genreRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ThrowsWhenInvalidGenreId))]
    public async Task ThrowsWhenInvalidGenreId()
    {
        var exampleIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var removedId = exampleIds[2];
        _genreRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleIds.FindAll(id => id != removedId));
        var input = _fixture.CreateValidCreateVideoInput(genresIds: exampleIds);

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related genre id (or ids) not found: {removedId}.");
        _genreRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(CreateVideoWithCastMembersIds))]
    public async Task CreateVideoWithCastMembersIds()
    {
        var exampleIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleIds);
        var input = _fixture.CreateValidCreateVideoInput(castMembersIds: exampleIds);

        var output = await _useCase.Handle(input, CancellationToken.None);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Categories.Should().BeEmpty();
        output.Genres.Should().BeEmpty();
        var outputItemCastMembersIds = output.CastMembers.Select(dto => dto.Id);
        outputItemCastMembersIds.Should().BeEquivalentTo(exampleIds);
        _videoRepositoryMock.Verify(x => x.Insert(
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
        _castMemberRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ThrowsWhenInvalidCastMemberId))]
    public async Task ThrowsWhenInvalidCastMemberId()
    {
        var exampleIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var removedId = exampleIds[2];
        _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleIds.FindAll(x => x != removedId));
        var input = _fixture.CreateValidCreateVideoInput(castMembersIds: exampleIds);

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related cast member id (or ids) not found: {removedId}.");
        _castMemberRepositoryMock.VerifyAll();
    }
}