using Codeflix.Catalog.Application.UseCases.Genres.Common;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Genres.UpdateGenre;
public interface IUpdateGenreUseCase : IRequestHandler<UpdateGenreInput, GenreModelOutput>
{
}
