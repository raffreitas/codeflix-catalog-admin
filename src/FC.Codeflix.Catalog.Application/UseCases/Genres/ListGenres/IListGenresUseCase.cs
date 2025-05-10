using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.ListGenres;

public interface IListGenresUseCase : IRequestHandler<ListGenresInput, ListGenresOutput>
{
}
