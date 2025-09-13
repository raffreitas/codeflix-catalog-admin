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
            await UploadImages(request, video, cancellationToken);

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
        if (request.BannerFile is not null && video.Banner is not null)
            await storageService.Delete(video.Banner.Path, cancellationToken);
        if (request.ThumbFile is not null && video.Thumb is not null)
            await storageService.Delete(video.Thumb.Path, cancellationToken);
        if (request.ThumbHalfFile is not null && video.ThumbHalf is not null)
            await storageService.Delete(video.ThumbHalf.Path, cancellationToken);
    }

    private async Task UploadImages(UploadMediasInput input, Video video, CancellationToken cancellationToken)
    {
        if (input.BannerFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Banner), input.BannerFile.Extension);
            var uploadedFilePath = await storageService.Upload(
                fileName,
                input.BannerFile.FileStream,
                input.BannerFile.ContentType,
                cancellationToken);
            video.UpdateBanner(uploadedFilePath);
        }

        if (input.ThumbFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Thumb), input.ThumbFile.Extension);
            var uploadedFilePath = await storageService.Upload(
                fileName,
                input.ThumbFile.FileStream,
                input.ThumbFile.ContentType,
                cancellationToken);
            video.UpdateThumb(uploadedFilePath);
        }

        if (input.ThumbHalfFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.ThumbHalf), input.ThumbHalfFile.Extension);
            var uploadedFilePath = await storageService.Upload(
                fileName,
                input.ThumbHalfFile.FileStream,
                input.ThumbHalfFile.ContentType,
                cancellationToken);
            video.UpdateThumbHalf(uploadedFilePath);
        }
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