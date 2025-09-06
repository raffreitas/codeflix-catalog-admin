using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Videos.UploadMedias;
using FC.Codeflix.Catalog.Domain.Repositories;

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
        var input = _fixture.GetValidInput();
        var exampleVideo = _fixture.GetValidVideo();
        _videoRepository.Setup(x => x.Get(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleVideo);
        _storageService.Setup(x => x.Upload(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(Guid.NewGuid().ToString());

        await _useCase.Handle(input, CancellationToken.None);

        _videoRepository.VerifyAll();
        _unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _storageService.Verify(x => x.Upload(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(2));
    }
}