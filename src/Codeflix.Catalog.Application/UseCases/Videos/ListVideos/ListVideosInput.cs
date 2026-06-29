using Codeflix.Catalog.Application.Common;

using Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Videos.ListVideos;

public sealed record ListVideosInput(
    int Page = 1,
    int PerPage = 15,
    string Search = "",
    string Sort = "",
    SearchOrder Dir = SearchOrder.Asc)
    : PaginatedListInput(Page, PerPage, Search, Sort, Dir), IRequest<ListVideosOutput>;