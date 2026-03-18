using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.ListVideos;

public sealed record ListVideosOutput(
    int Page,
    int PerPage,
    int Total,
    IReadOnlyList<VideoModelOutput> Items)
    : PaginatedListOutput<VideoModelOutput>(Page, PerPage, Total, Items);