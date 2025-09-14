using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;
using FC.Codeflix.Catalog.Application.UseCases.Videos.UpdateMediaStatus;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.Domain.Repositories;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Videos.UpdateMediaStatus;

public sealed class UpdateMediaStatusTest : IClassFixture<UpdateMediaStatusTestFixture>
{
    private readonly Mock<IVideoRepository> _videoRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly UpdateMediaStatusUseCase _useCase;
    private readonly UpdateMediaStatusTestFixture _fixture;

    public UpdateMediaStatusTest(UpdateMediaStatusTestFixture fixture)
    {
        _fixture = fixture;
        _useCase = new UpdateMediaStatusUseCase(
            _videoRepository.Object,
            _unitOfWork.Object,
            Mock.Of<ILogger<UpdateMediaStatusUseCase>>()
        );
    }

    [Fact(DisplayName = nameof(HandleWhenSucceededEncoding))]
    [Trait("Application", "UpdateMediaStatus - Use Cases")]
    public async Task HandleWhenSucceededEncoding()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.GetSucceededEncodingInput(exampleVideo.Id);
        _videoRepository.Setup(x => x.Get(
                exampleVideo.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(exampleVideo.Title);
        output.Description.Should().Be(exampleVideo.Description);
        output.Published.Should().Be(exampleVideo.Published);
        output.Opened.Should().Be(exampleVideo.Opened);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.Rating.Should().Be(exampleVideo.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        exampleVideo.Media!.Status.Should().Be(MediaStatus.Completed);
        exampleVideo.Media!.EncodedPath.Should().Be(input.EncodedPath);
        _videoRepository.VerifyAll();
        _videoRepository.Verify(x => x.Update(
            exampleVideo, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(HandleWhenFailedEncoding))]
    [Trait("Application", "UpdateMediaStatus - Use Cases")]
    public async Task HandleWhenFailedEncoding()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.GetFailedEncodingInput(exampleVideo.Id);
        _videoRepository.Setup(x => x.Get(
                exampleVideo.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(exampleVideo.Title);
        output.Description.Should().Be(exampleVideo.Description);
        output.Published.Should().Be(exampleVideo.Published);
        output.Opened.Should().Be(exampleVideo.Opened);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.Rating.Should().Be(exampleVideo.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        exampleVideo.Media!.Status.Should().Be(MediaStatus.Error);
        exampleVideo.Media!.EncodedPath.Should().BeNull();
        _videoRepository.VerifyAll();
        _videoRepository.Verify(x => x.Update(
            exampleVideo, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(HandleWhenInvalidInput))]
    [Trait("Application", "UpdateMediaStatus - Use Cases")]
    public async Task HandleWhenInvalidInput()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var expectedEncodedPath = exampleVideo.Media!.EncodedPath;
        var expectedStatus = exampleVideo.Media.Status;
        const string expectedErrorMessage = "Invalid media status";
        var input = _fixture.GetInvalidStatusInput(exampleVideo.Id);
        _videoRepository.Setup(x => x.Get(
                exampleVideo.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var action = async () => await _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<EntityValidationException>()
            .WithMessage(expectedErrorMessage);
        exampleVideo.Media!.Status.Should().Be(expectedStatus);
        exampleVideo.Media!.EncodedPath.Should().Be(expectedEncodedPath);
        _videoRepository.VerifyAll();
        _videoRepository.Verify(x => x.Update(
            exampleVideo, It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }
}