using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Videos.ListVideos;

public interface IListVideosUseCase : IRequestHandler<ListVideosInput, ListVideosOutput>
{
}