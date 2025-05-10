using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.DeleteGenre;
public record DeleteGenreInput(Guid Id) : IRequest
{
}
