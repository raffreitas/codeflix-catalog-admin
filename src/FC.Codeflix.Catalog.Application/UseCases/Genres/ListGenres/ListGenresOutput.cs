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

    public static ListGenresOutput FromSearchOutput(SearchOutput<Genre> searchOutput)
        => new(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items
                .Select(GenreModelOutput.FromGenre)
                .ToList()
        );

    internal void FillWithCategoryNames(IReadOnlyList<Category> categories)
    {
        foreach (GenreModelOutput item in Items)
        foreach (GenreModelOutputCategory categoryOutput in item.Categories)
            categoryOutput.Name = categories?.FirstOrDefault(category => category.Id == categoryOutput.Id
            )?.Name;
    }
}