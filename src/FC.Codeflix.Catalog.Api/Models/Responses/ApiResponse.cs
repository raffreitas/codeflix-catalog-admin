namespace FC.Codeflix.Catalog.Api.Models.Responses;

public record ApiResponse<TData>
{
    public ApiResponse(TData data)
    {
        Data = data;
    }

    public TData Data { get; set; }
}
