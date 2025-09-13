using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repositories;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.DeleteVideo;

public sealed class DeleteVideoUseCase(
    IVideoRepository videoRepository,
    IUnitOfWork unitOfWork,
    IStorageService storageService) : IDeleteVideoUseCase
{
    public async Task Handle(DeleteVideoInput request, CancellationToken cancellationToken)
    {
        var video = await videoRepository.Get(request.VideoId, cancellationToken);
        var trailerFilePath = video.Trailer?.FilePath;
        var mediaFilePath = video.Media?.FilePath;

        await videoRepository.Delete(video, cancellationToken);
        await unitOfWork.Commit(cancellationToken);

        await ClearVideoMedias(
            mediaFilePath,
            trailerFilePath,
            cancellationToken);

        await ClearImageMedias(
            video.Banner?.Path,
            video.Thumb?.Path,
            video.ThumbHalf?.Path,
            cancellationToken);
    }

    private async Task ClearImageMedias(
        string? bannerFilePath,
        string? thumbFilePath,
        string? thumbHalfFilePath,
        CancellationToken cancellationToken)
    {
        if (bannerFilePath is not null)
            await storageService.Delete(bannerFilePath, cancellationToken);

        if (thumbFilePath is not null)
            await storageService.Delete(thumbFilePath, cancellationToken);

        if (thumbHalfFilePath is not null)
            await storageService.Delete(thumbHalfFilePath, cancellationToken);
    }

    private async Task ClearVideoMedias(
        string? mediaFilePath,
        string? trailerFilePath,
        CancellationToken cancellationToken)
    {
        if (trailerFilePath is not null)
            await storageService.Delete(trailerFilePath, cancellationToken);

        if (mediaFilePath is not null)
            await storageService.Delete(mediaFilePath, cancellationToken);
    }
}