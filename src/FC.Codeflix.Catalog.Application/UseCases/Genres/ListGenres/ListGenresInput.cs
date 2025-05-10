using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.ListGenres;
public record ListGenresInput : PaginatedListInput, IRequest<ListGenresOutput>
{
    public ListGenresInput(
        int page = 1, 
        int perPage= 15, 
        string search = "", 
        string sort = "", 
        SearchOrder dir = SearchOrder.Asc
    ) : base(page, perPage, search, sort, dir)
    {
    }
}
