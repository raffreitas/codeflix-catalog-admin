using FC.Codeflix.Catalog.Application.Common;

namespace FC.Codeflix.Catalog.Api.Models.Responses;

public record TestApiResponseList<TItemData> : ApiResponse<IReadOnlyList<TItemData>>
{
    public ApiResponseListMeta Meta { get; private set; }
    public TestApiResponseList(int currentPage, int perPage, int total, IReadOnlyList<TItemData> original)
        : base(original)
    {
        Meta = new(currentPage, perPage, total);
    }

    public TestApiResponseList(PaginatedListOutput<TItemData> paginatedListOutput)
        : base(paginatedListOutput.Items)
    {
        Meta = new(paginatedListOutput.Page, paginatedListOutput.PerPage, paginatedListOutput.Total);
    }
}
