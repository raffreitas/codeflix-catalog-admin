namespace FC.Codeflix.Catalog.Api.Models.Responses;

public record ApiResponseListMeta
{
    public ApiResponseListMeta(int currentPage, int perPage, int total)
    {
        CurrentPage = currentPage;
        PerPage = perPage;
        Total = total;
    }

    public int CurrentPage { get; set; }
    public int PerPage { get; set; }
    public int Total { get; set; }
}
