using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.ListGenres;
public record ListGenresInput : PaginatedListInput, IRequest<ListGenresOutput>
{
    public ListGenresInput(
        int page, 
        int perPage, 
        string search, 
        string sort, 
        SearchOrder dir
    ) : base(page, perPage, search, sort, dir)
    {
    }
}
