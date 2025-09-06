using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Videos.UploadMedias;
using FC.Codeflix.Catalog.Domain.Repositories;

using FluentAssertions;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Videos.UploadMedias;

[Trait("Application", "UploadMedia - Use Cases")]
public sealed class UploadMediasTest : IClassFixture<UploadMediasTestFixture>
{
    private readonly Mock<IVideoRepository> _videoRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IStorageService> _storageService = new();

    private readonly UploadMediasTestFixture _fixture;
    private readonly UploadMediasUseCase _useCase;

    public UploadMediasTest(UploadMediasTestFixture fixture)
    {
        _fixture = fixture;
        _useCase = new UploadMediasUseCase(_videoRepository.Object, _unitOfWork.Object, _storageService.Object);
    }

    [Fact(DisplayName = nameof(UploadMedias))]
    public async Task UploadMedias()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.GetValidInput(exampleVideo.Id);
        string[] fileNames =
        [
            StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.Media), input.VideoFile!.Extension),
            StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.Trailer), input.TrailerFile!.Extension)
        ];
        _videoRepository.Setup(x => x.Get(
            It.Is<Guid>(id => id == input.VideoId),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleVideo);
        _storageService.Setup(x => x.Upload(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(Guid.NewGuid().ToString());

        await _useCase.Handle(input, CancellationToken.None);

        _videoRepository.VerifyAll();
        _unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _storageService.Verify(x => x.Upload(
            It.Is<string>(fileName => fileNames.Contains(fileName)),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(2));
    }

    [Fact(DisplayName = nameof(ThrowsWhenVideoNotFound))]
    public async Task ThrowsWhenVideoNotFound()
    {
        var video = _fixture.GetValidVideo();
        var input = _fixture.GetValidInput(video.Id);
        _videoRepository.Setup(x => x.Get(
            It.Is<Guid>(id => id == video.Id),
            It.IsAny<CancellationToken>())
        ).ThrowsAsync(new NotFoundException("Video not found."));

        var act = async () => await _useCase.Handle(input, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Video not found.");
    }

    [Fact(DisplayName = nameof(ClearStorageInUploadErrorCase))]
    public async Task ClearStorageInUploadErrorCase()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.GetValidInput(exampleVideo.Id);
        var videoFileName = StorageFileName
            .Create(exampleVideo.Id, nameof(exampleVideo.Media), input.VideoFile!.Extension);
        var trailerFileName = StorageFileName
            .Create(exampleVideo.Id, nameof(exampleVideo.Trailer), input.TrailerFile!.Extension);
        string[] fileNames = [videoFileName, trailerFileName];
        _videoRepository.Setup(x => x.Get(
            It.Is<Guid>(id => id == input.VideoId),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleVideo);
        _storageService.Setup(x => x.Upload(
            It.Is<string>(name => name == videoFileName),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(videoFileName);
        _storageService.Setup(x => x.Upload(
            It.Is<string>(name => name == trailerFileName),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>())
        ).ThrowsAsync(new Exception("Something went wrong when trying to upload the file."));

        var act = async () => await _useCase.Handle(input, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Something went wrong when trying to upload the file.");

        _videoRepository.VerifyAll();
        _unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
        _storageService.Verify(x => x.Upload(
            It.Is<string>(fileName => fileNames.Contains(fileName)),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(2));
        _storageService.Verify(x => x.Delete(
            It.Is<string>(fileName => fileName == videoFileName),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact(DisplayName = nameof(ClearStorageInCommitErrorCase))]
    public async Task ClearStorageInCommitErrorCase()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.GetValidInput(exampleVideo.Id);
        var videoFileName = StorageFileName
            .Create(exampleVideo.Id, nameof(exampleVideo.Media), input.VideoFile!.Extension);
        var trailerFileName = StorageFileName
            .Create(exampleVideo.Id, nameof(exampleVideo.Trailer), input.TrailerFile!.Extension);
        string[] fileNames = [videoFileName, trailerFileName];
        _videoRepository.Setup(x => x.Get(
            It.Is<Guid>(id => id == input.VideoId),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleVideo);
        _storageService.Setup(x => x.Upload(
            It.Is<string>(name => name == videoFileName),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(videoFileName);
        _storageService.Setup(x => x.Upload(
            It.Is<string>(name => name == trailerFileName),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(trailerFileName);
        _unitOfWork.Setup(x => x.Commit(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong when trying to commit the transaction."));

        var act = async () => await _useCase.Handle(input, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Something went wrong when trying to commit the transaction.");

        _videoRepository.VerifyAll();
        _storageService.Verify(x => x.Upload(
            It.Is<string>(fileName => fileNames.Contains(fileName)),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(2));
        _storageService.Verify(x => x.Delete(
            It.Is<string>(fileName => fileNames.Contains(fileName)),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(2));
    }

    [Fact(DisplayName = nameof(ClearOnlyOneFileStorageInCommitErrorCaseIfProvidedOnlyOneFile))]
    public async Task ClearOnlyOneFileStorageInCommitErrorCaseIfProvidedOnlyOneFile()
    {
        var video = _fixture.GetValidVideo();
        video.UpdateTrailer(_fixture.GetValidMediaPath());
        video.UpdateMedia(_fixture.GetValidMediaPath());
        var input = _fixture.GetValidInput(video.Id, withTrailerFile: false);
        var videoFileName = StorageFileName
            .Create(video.Id, nameof(video.Media), input.VideoFile!.Extension);
        _videoRepository.Setup(x => x.Get(
            It.Is<Guid>(id => id == input.VideoId),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(video);
        _storageService.Setup(x => x.Upload(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(Guid.NewGuid().ToString());
        _storageService.Setup(x => x.Upload(
            It.Is<string>(name => name == videoFileName),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(videoFileName);
        _unitOfWork.Setup(x => x.Commit(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong when trying to commit the transaction."));

        var act = async () => await _useCase.Handle(input, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Something went wrong when trying to commit the transaction.");

        _videoRepository.VerifyAll();
        _storageService.Verify(x => x.Upload(
            It.Is<string>(fileName => fileName == videoFileName),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        _storageService.Verify(x => x.Upload(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        _storageService.Verify(x => x.Delete(
            It.Is<string>(fileName => fileName == videoFileName),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}