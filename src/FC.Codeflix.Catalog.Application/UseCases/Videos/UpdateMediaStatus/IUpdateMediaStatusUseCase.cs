using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.UpdateMediaStatus;

public interface IUpdateMediaStatusUseCase : IRequestHandler<UpdateMediaStatusInput, VideoModelOutput>;