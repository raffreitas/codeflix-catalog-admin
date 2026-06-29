using Codeflix.Catalog.Application.UseCases.Videos.Common;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Videos.UpdateMediaStatus;

public interface IUpdateMediaStatusUseCase : IRequestHandler<UpdateMediaStatusInput, VideoModelOutput>;