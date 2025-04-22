using FC.Codeflix.Catalog.Application.UseCases.Genres.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.UpdateGenre;
public interface IUpdateGenreUseCase : IRequestHandler<UpdateGenreInput, GenreModelOutput>
{
}
