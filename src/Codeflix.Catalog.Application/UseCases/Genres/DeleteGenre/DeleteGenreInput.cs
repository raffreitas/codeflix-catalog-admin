using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Genres.DeleteGenre;
public record DeleteGenreInput(Guid Id) : IRequest
{
}
