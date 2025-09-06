using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.ListVideos;

public sealed record ListVideosInput : PaginatedListInput, IRequest<ListVideosOutput>
{
    public ListVideosInput(
        int page,
        int perPage,
        string search,
        string sort,
        SearchOrder dir)
        : base(page, perPage, search, sort, dir)
    {
    }
}