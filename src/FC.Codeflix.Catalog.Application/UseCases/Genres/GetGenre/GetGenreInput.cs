using FC.Codeflix.Catalog.Application.UseCases.Genres.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.GetGenre;
public record GetGenreInput(Guid Id) : IRequest<GenreModelOutput>
{
}
