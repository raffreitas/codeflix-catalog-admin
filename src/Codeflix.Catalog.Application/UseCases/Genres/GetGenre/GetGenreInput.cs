using Codeflix.Catalog.Application.UseCases.Genres.Common;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Genres.GetGenre;
public record GetGenreInput(Guid Id) : IRequest<GenreModelOutput>
{
}
