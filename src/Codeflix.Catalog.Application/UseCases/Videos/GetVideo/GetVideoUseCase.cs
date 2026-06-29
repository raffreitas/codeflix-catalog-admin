using Codeflix.Catalog.Application.UseCases.Videos.Common;

using Codeflix.Catalog.Domain.Repositories;

namespace Codeflix.Catalog.Application.UseCases.Videos.GetVideo;

public sealed class GetVideoUseCase(IVideoRepository videoRepository) : IGetVideoUseCase
{
    public async Task<VideoModelOutput> Handle(GetVideoInput request, CancellationToken cancellationToken)
    {
        var video = await videoRepository.Get(request.VideoId, cancellationToken);
        return VideoModelOutput.FromVideo(video);
    }
}