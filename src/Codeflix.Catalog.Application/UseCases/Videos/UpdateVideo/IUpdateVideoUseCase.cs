using Codeflix.Catalog.Application.UseCases.Videos.Common;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Videos.UpdateVideo;

public interface IUpdateVideoUseCase : IRequestHandler<UpdateVideoInput, VideoModelOutput>
{
}