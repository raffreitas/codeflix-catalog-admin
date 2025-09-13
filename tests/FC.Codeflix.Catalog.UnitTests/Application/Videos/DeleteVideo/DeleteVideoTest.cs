using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Videos.DeleteVideo;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Repositories;

using FluentAssertions;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Videos.DeleteVideo;

[Trait("Application", "DeleteVideo - Use Cases")]
public sealed class DeleteVideoTest : IClassFixture<DeleteVideoTestFixture>
{
    private readonly Mock<IVideoRepository> _repositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IStorageService> _storageServiceMock = new();

    private readonly DeleteVideoTestFixture _fixture;
    private readonly DeleteVideoUseCase _useCase;

    public DeleteVideoTest(DeleteVideoTestFixture fixture)
    {
        _fixture = fixture;
        _useCase = new DeleteVideoUseCase(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _storageServiceMock.Object
        );
    }

    [Fact(DisplayName = nameof(DeleteVideo))]
    public async Task DeleteVideo()
    {
        var videoExample = _fixture.GetValidVideo();
        var input = _fixture.GetValidInput(videoExample.Id);
        _repositoryMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == videoExample.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(videoExample);

        await _useCase.Handle(input, CancellationToken.None);

        _repositoryMock.VerifyAll();
        _repositoryMock.Verify(x => x.Delete(
            It.Is<Video>(video => video.Id == videoExample.Id),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(DeleteVideoWithAllMediasAndClearStorage))]
    public async Task DeleteVideoWithAllMediasAndClearStorage()
    {
        var videoExample = _fixture.GetValidVideo();
        videoExample.UpdateMedia(_fixture.GetValidMediaPath());
        videoExample.UpdateTrailer(_fixture.GetValidMediaPath());
        videoExample.UpdateBanner(_fixture.GetValidImagePath());
        videoExample.UpdateThumb(_fixture.GetValidImagePath());
        videoExample.UpdateThumbHalf(_fixture.GetValidImagePath());
        var filePaths = new List<string>
        {
            videoExample.Media!.FilePath,
            videoExample.Trailer!.FilePath,
            videoExample.Banner!.Path,
            videoExample.Thumb!.Path,
            videoExample.ThumbHalf!.Path,
        };
        var input = _fixture.GetValidInput(videoExample.Id);
        _repositoryMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == videoExample.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(videoExample);

        await _useCase.Handle(input, CancellationToken.None);

        _repositoryMock.VerifyAll();
        _repositoryMock.Verify(x => x.Delete(
                It.Is<Video>(video => video.Id == videoExample.Id),
                It.IsAny<CancellationToken>())
            , Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        _storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(filePath => filePaths.Contains(filePath)),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(5));
        _storageServiceMock.Verify(x => x.Delete(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(5));
    }


    [Fact(DisplayName = nameof(DeleteVideoWithOnlyTrailerAndClearStorageOnlyForTrailer))]
    public async Task DeleteVideoWithOnlyTrailerAndClearStorageOnlyForTrailer()
    {
        var videoExample = _fixture.GetValidVideo();
        videoExample.UpdateTrailer(_fixture.GetValidMediaPath());
        var input = _fixture.GetValidInput(videoExample.Id);
        _repositoryMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == videoExample.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(videoExample);

        await _useCase.Handle(input, CancellationToken.None);

        _repositoryMock.VerifyAll();
        _repositoryMock.Verify(x => x.Delete(
                It.Is<Video>(video => video.Id == videoExample.Id),
                It.IsAny<CancellationToken>())
            , Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        _storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(filePath => filePath == videoExample.Trailer!.FilePath),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(1));
        _storageServiceMock.Verify(x => x.Delete(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(1));
    }

    [Fact(DisplayName = nameof(DeleteVideoWithOnlyBannerAndClearStorageOnlyForBanner))]
    [Trait("Application", "DeleteVideo - Use Cases")]
    public async Task DeleteVideoWithOnlyBannerAndClearStorageOnlyForBanner()
    {
        var videoExample = _fixture.GetValidVideo();
        videoExample.UpdateBanner(_fixture.GetValidImagePath());
        var input = _fixture.GetValidInput(videoExample.Id);
        _repositoryMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == videoExample.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(videoExample);

        await _useCase.Handle(input, CancellationToken.None);

        _repositoryMock.VerifyAll();
        _repositoryMock.Verify(x => x.Delete(
                It.Is<Video>(video => video.Id == videoExample.Id),
                It.IsAny<CancellationToken>())
            , Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        _storageServiceMock.Verify(x => x.Delete(
                It.Is<string>(filePath => filePath == videoExample.Banner!.Path),
                It.IsAny<CancellationToken>())
            , Times.Exactly(1));
        _storageServiceMock.Verify(x => x.Delete(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>())
            , Times.Exactly(1));
    }

    [Fact(DisplayName = nameof(DeleteVideoWithOnlyThumbAndClearStorageOnlyForThumb))]
    [Trait("Application", "DeleteVideo - Use Cases")]
    public async Task DeleteVideoWithOnlyThumbAndClearStorageOnlyForThumb()
    {
        var videoExample = _fixture.GetValidVideo();
        videoExample.UpdateThumb(_fixture.GetValidImagePath());
        var input = _fixture.GetValidInput(videoExample.Id);
        _repositoryMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == videoExample.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(videoExample);

        await _useCase.Handle(input, CancellationToken.None);

        _repositoryMock.VerifyAll();
        _repositoryMock.Verify(x => x.Delete(
                It.Is<Video>(video => video.Id == videoExample.Id),
                It.IsAny<CancellationToken>())
            , Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        _storageServiceMock.Verify(x => x.Delete(
                It.Is<string>(filePath => filePath == videoExample.Thumb!.Path),
                It.IsAny<CancellationToken>())
            , Times.Exactly(1));
        _storageServiceMock.Verify(x => x.Delete(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>())
            , Times.Exactly(1));
    }

    [Fact(DisplayName = nameof(DeleteVideoWithOnlyThumbHalfAndClearStorageOnlyForThumbHalf))]
    [Trait("Application", "DeleteVideo - Use Cases")]
    public async Task DeleteVideoWithOnlyThumbHalfAndClearStorageOnlyForThumbHalf()
    {
        var videoExample = _fixture.GetValidVideo();
        videoExample.UpdateThumbHalf(_fixture.GetValidImagePath());
        var input = _fixture.GetValidInput(videoExample.Id);
        _repositoryMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == videoExample.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(videoExample);

        await _useCase.Handle(input, CancellationToken.None);

        _repositoryMock.VerifyAll();
        _repositoryMock.Verify(x => x.Delete(
                It.Is<Video>(video => video.Id == videoExample.Id),
                It.IsAny<CancellationToken>())
            , Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        _storageServiceMock.Verify(x => x.Delete(
                It.Is<string>(filePath => filePath == videoExample.ThumbHalf!.Path),
                It.IsAny<CancellationToken>())
            , Times.Exactly(1));
        _storageServiceMock.Verify(x => x.Delete(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>())
            , Times.Exactly(1));
    }

    [Fact(DisplayName = nameof(DeleteVideoWithOnlyMediaAndClearStorageOnlyForMedia))]
    public async Task DeleteVideoWithOnlyMediaAndClearStorageOnlyForMedia()
    {
        var videoExample = _fixture.GetValidVideo();
        videoExample.UpdateMedia(_fixture.GetValidMediaPath());
        var input = _fixture.GetValidInput(videoExample.Id);
        _repositoryMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == videoExample.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(videoExample);

        await _useCase.Handle(input, CancellationToken.None);

        _repositoryMock.VerifyAll();
        _repositoryMock.Verify(x => x.Delete(
            It.Is<Video>(video => video.Id == videoExample.Id),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        _storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(filePath => filePath == videoExample.Media!.FilePath),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(1));
        _storageServiceMock.Verify(x => x.Delete(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(1));
    }

    [Fact(DisplayName = nameof(DeleteVideoWithoutAnyMediaAndDontClearStorage))]
    public async Task DeleteVideoWithoutAnyMediaAndDontClearStorage()
    {
        var videoExample = _fixture.GetValidVideo();
        var input = _fixture.GetValidInput(videoExample.Id);
        _repositoryMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == videoExample.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(videoExample);

        await _useCase.Handle(input, CancellationToken.None);

        _repositoryMock.VerifyAll();
        _repositoryMock.Verify(x => x.Delete(
            It.Is<Video>(video => video.Id == videoExample.Id),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        _storageServiceMock.Verify(x => x.Delete(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }

    [Fact(DisplayName = nameof(ThrowsNotFoundExceptionWhenVideoNotFound))]
    public async Task ThrowsNotFoundExceptionWhenVideoNotFound()
    {
        var input = _fixture.GetValidInput();
        _repositoryMock.Setup(x => x.Get(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(new NotFoundException("Video not found"));

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Video not found");

        _repositoryMock.VerifyAll();
        _repositoryMock.Verify(x => x.Delete(
            It.IsAny<Video>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
        _storageServiceMock.Verify(x => x.Delete(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }
}