using FC.Codeflix.Catalog.Application.UseCases.Genres.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.GetGenre;

public interface IGetGenreUseCase : IRequestHandler<GetGenreInput, GenreModelOutput>
{
}