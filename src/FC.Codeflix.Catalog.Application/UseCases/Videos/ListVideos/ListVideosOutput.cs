﻿using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.ListVideos;

public sealed record ListVideosOutput : PaginatedListOutput<VideoModelOutput>
{
    public ListVideosOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<VideoModelOutput> items)
        : base(page, perPage, total, items)
    {
    }
}