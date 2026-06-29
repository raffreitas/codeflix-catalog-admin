using Codeflix.Catalog.Application.UseCases.Videos.Common;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Videos.GetVideo;

public interface IGetVideoUseCase : IRequestHandler<GetVideoInput, VideoModelOutput>
{
}