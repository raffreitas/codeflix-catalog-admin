using FC.Codeflix.Catalog.Domain.Repositories;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.ListGenres;

public class ListGenresUseCase : IListGenresUseCase
{
    private readonly IGenreRepository _genreRepository;

    public ListGenresUseCase(IGenreRepository genreRepository)
    {
        _genreRepository = genreRepository;
    }

    public async Task<ListGenresOutput> Handle(ListGenresInput request, CancellationToken cancellationToken)
    {
        var searchOutput = await _genreRepository.Search(request.ToSearchInput(), cancellationToken);
        return ListGenresOutput.FromSearchOutput(searchOutput);
    }
}
