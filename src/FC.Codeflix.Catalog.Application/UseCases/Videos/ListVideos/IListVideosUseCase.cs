using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.ListVideos;

public interface IListVideosUseCase : IRequestHandler<ListVideosInput, ListVideosOutput>
{
}