using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Genres.ListGenres;

public interface IListGenresUseCase : IRequestHandler<ListGenresInput, ListGenresOutput>
{
}
