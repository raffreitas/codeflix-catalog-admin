using Codeflix.Catalog.Application.UseCases.Genres.Common;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Genres.GetGenre;

public interface IGetGenreUseCase : IRequestHandler<GetGenreInput, GenreModelOutput>
{
}