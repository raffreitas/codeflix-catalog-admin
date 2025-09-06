using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.GetVideo;

public interface IGetVideoUseCase : IRequestHandler<GetVideoInput, VideoModelOutput>
{
}