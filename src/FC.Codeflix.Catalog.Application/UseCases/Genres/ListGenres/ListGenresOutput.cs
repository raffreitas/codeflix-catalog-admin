
using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genres.Common;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

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

    public static ListGenresOutput FromSearchOutput(
        SearchOutput<Genre> searchOutput
    ) => new(
            page: searchOutput.CurrentPage,
            perPage: searchOutput.PerPage,
            total: searchOutput.Total,
            items: [.. searchOutput.Items.Select(GenreModelOutput.FromGenre)]
        );
}
