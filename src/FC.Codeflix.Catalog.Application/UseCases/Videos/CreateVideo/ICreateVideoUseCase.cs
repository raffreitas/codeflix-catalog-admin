using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.CreateVideo;

public interface ICreateVideoUseCase : IRequestHandler<CreateVideoInput, VideoModelOutput>
{
}