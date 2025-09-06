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

        await videoRepository.Delete(video, cancellationToken);
        await unitOfWork.Commit(cancellationToken);

        if (video.Trailer is not null)
            await storageService.Delete(video.Trailer.FilePath, cancellationToken);

        if (video.Media is not null)
            await storageService.Delete(video.Media.FilePath, cancellationToken);
    }
}