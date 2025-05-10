using FC.Codeflix.Catalog.Application.UseCases.Genres.Common;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

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
        var searchInput = new SearchInput(
            page: request.Page,
            perPage: request.PerPage,
            search: request.Search,
            orderBy: request.Sort,
            order: request.Dir
        );
        var searchOutput = await _genreRepository.Search(searchInput, cancellationToken);
        return new ListGenresOutput(
            page: searchOutput.CurrentPage,
            perPage: searchOutput.PerPage,
            total: searchOutput.Total,
            items: [.. searchOutput.Items.Select(GenreModelOutput.FromGenre)]
        );
    }
}
