using Codeflix.Catalog.Application.Common;
using Codeflix.Catalog.Application.UseCases.Genres.Common;

using Codeflix.Catalog.Domain.Entities;
using Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

namespace Codeflix.Catalog.Application.UseCases.Genres.ListGenres;

public record ListGenresOutput(
    int Page,
    int PerPage,
    int Total,
    IReadOnlyList<GenreModelOutput> Items)
    : PaginatedListOutput<GenreModelOutput>(Page, PerPage, Total, Items)
{
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