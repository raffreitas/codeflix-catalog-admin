using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.UpdateVideo;

public interface IUpdateVideoUseCase : IRequestHandler<UpdateVideoInput, VideoModelOutput>
{
}