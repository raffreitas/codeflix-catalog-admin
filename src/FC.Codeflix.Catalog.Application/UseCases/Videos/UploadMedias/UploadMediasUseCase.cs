using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Repositories;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.UploadMedias;

public sealed class UploadMediasUseCase(
    IVideoRepository videoRepository,
    IUnitOfWork unitOfWork,
    IStorageService storageService) : IUploadMediasUseCase
{
    public async Task Handle(UploadMediasInput request, CancellationToken cancellationToken)
    {
        var video = await videoRepository.Get(request.VideoId, cancellationToken);
        try
        {
            await UploadVideo(request, video, cancellationToken);
            await UploadTrailer(request, video, cancellationToken);

            await videoRepository.Update(video, cancellationToken);
            await unitOfWork.Commit(cancellationToken);
        }
        catch (Exception)
        {
            await ClearStorage(request, video, cancellationToken);
            throw;
        }
    }

    private async Task ClearStorage(UploadMediasInput request, Video video, CancellationToken cancellationToken)
    {
        if (request.VideoFile is not null && video.Media is not null)
            await storageService.Delete(video.Media.FilePath, cancellationToken);
        if (request.TrailerFile is not null && video.Trailer is not null)
            await storageService.Delete(video.Trailer.FilePath, cancellationToken);
    }

    private async Task UploadTrailer(UploadMediasInput request, Video video, CancellationToken cancellationToken)
    {
        if (request.TrailerFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Trailer), request.TrailerFile.Extension);
            var uploadedFilePath = await storageService.Upload(
                fileName,
                request.TrailerFile.FileStream,
                request.TrailerFile.ContentType,
                cancellationToken
            );

            video.UpdateTrailer(uploadedFilePath);
        }
    }

    private async Task UploadVideo(UploadMediasInput request, Video video, CancellationToken cancellationToken)
    {
        if (request.VideoFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Media), request.VideoFile.Extension);
            var uploadedFilePath = await storageService.Upload(
                fileName,
                request.VideoFile.FileStream,
                request.VideoFile.ContentType,
                cancellationToken
            );

            video.UpdateMedia(uploadedFilePath);
        }
    }
}