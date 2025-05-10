
using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genres.Common;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.ListGenres;

public record ListGenresOutput : PaginatedListOutput<GenreModelOutput>
{
    public ListGenresOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<GenreModelOutput> items
    ) : base(page, perPage, total, items)
    {
    }
}
