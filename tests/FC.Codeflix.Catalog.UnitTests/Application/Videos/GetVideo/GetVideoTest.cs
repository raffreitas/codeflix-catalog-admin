using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Videos.GetVideo;
using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.Domain.Repositories;

using FluentAssertions;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Videos.GetVideo;

[Trait("Application", "GetVideo - Use Cases")]
public sealed class GetVideoTest : IClassFixture<GetVideoTestFixture>
{
    private readonly Mock<IVideoRepository> _videoRepositoryMock = new();

    private readonly GetVideoTestFixture _fixture;
    private readonly GetVideoUseCase _useCase;

    public GetVideoTest(GetVideoTestFixture fixture)
    {
        _fixture = fixture;
        _useCase = new GetVideoUseCase(_videoRepositoryMock.Object);
    }

    [Fact(DisplayName = nameof(Get))]
    public async Task Get()
    {
        var exampleVideo = _fixture.GetValidVideo();
        _videoRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == exampleVideo.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleVideo);
        var input = new GetVideoInput(exampleVideo.Id);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.Title.Should().Be(exampleVideo.Title);
        output.Published.Should().Be(exampleVideo.Published);
        output.Description.Should().Be(exampleVideo.Description);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.Rating.Should().Be(exampleVideo.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        output.Opened.Should().Be(exampleVideo.Opened);
        _videoRepositoryMock.VerifyAll();
    }


    [Fact(DisplayName = nameof(GetWithAllProperties))]
    [Trait("Application", "GetVideo - Use Cases")]
    public async Task GetWithAllProperties()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();

        _videoRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == exampleVideo.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleVideo);
        var input = new GetVideoInput(exampleVideo.Id);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.Title.Should().Be(exampleVideo.Title);
        output.Published.Should().Be(exampleVideo.Published);
        output.Description.Should().Be(exampleVideo.Description);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.Rating.Should().Be(exampleVideo.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        output.Opened.Should().Be(exampleVideo.Opened);
        output.ThumbFileUrl.Should().Be(exampleVideo.Thumb!.Path);
        output.ThumbHalfFileUrl.Should().Be(exampleVideo.ThumbHalf!.Path);
        output.BannerFileUrl.Should().Be(exampleVideo.Banner!.Path);
        output.VideoFileUrl.Should().Be(exampleVideo.Media!.FilePath);
        output.TrailerFileUrl.Should().Be(exampleVideo.Trailer!.FilePath);
        var outputItemCategoryIds = output.Categories
            .Select(categoryDto => categoryDto.Id).ToList();
        outputItemCategoryIds.Should().BeEquivalentTo(exampleVideo.Categories);
        var outputItemGenresIds = output.Genres
            .Select(dto => dto.Id).ToList();
        outputItemGenresIds.Should().BeEquivalentTo(exampleVideo.Genres);
        var outputItemCastMembersIds = output.CastMembers
            .Select(dto => dto.Id).ToList();
        outputItemCastMembersIds.Should().BeEquivalentTo(exampleVideo.CastMembers);
        _videoRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ThrowsExceptionWhenNotFound))]
    public async Task ThrowsExceptionWhenNotFound()
    {
        _videoRepositoryMock.Setup(x => x.Get(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>())
        ).ThrowsAsync(new NotFoundException("Video not found"));
        var input = new GetVideoInput(Guid.NewGuid());

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Video not found");
        _videoRepositoryMock.VerifyAll();
    }
}