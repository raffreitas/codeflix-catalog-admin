using Codeflix.Catalog.Application.Common;
using Codeflix.Catalog.Application.UseCases.Videos.Common;

namespace Codeflix.Catalog.Application.UseCases.Videos.ListVideos;

public sealed record ListVideosOutput(
    int Page,
    int PerPage,
    int Total,
    IReadOnlyList<VideoModelOutput> Items)
    : PaginatedListOutput<VideoModelOutput>(Page, PerPage, Total, Items);