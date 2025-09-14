using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repositories;

using Microsoft.Extensions.Logging;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.UpdateMediaStatus;

public sealed class UpdateMediaStatusUseCase(
    IVideoRepository videoRepository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateMediaStatusUseCase> logger) : IUpdateMediaStatusUseCase
{
    public async Task<VideoModelOutput> Handle(UpdateMediaStatusInput request, CancellationToken cancellationToken)
    {
        var video = await videoRepository.Get(request.VideoId, cancellationToken);

        switch (request.Status)
        {
            case MediaStatus.Completed:
                video.UpdateAsEncoded(request.EncodedPath!);
                break;
            case MediaStatus.Error:
                logger.LogError(
                    "There was an error while trying to encode the video {VideoId}: {Error}",
                    video.Id, request.ErrorMessage
                );
                video.UpdateAsEncodingError();
                break;
            default:
                throw new EntityValidationException("Invalid media status");
        }

        await videoRepository.Update(video, cancellationToken);
        await unitOfWork.Commit(cancellationToken);
        return VideoModelOutput.FromVideo(video);
    }
}