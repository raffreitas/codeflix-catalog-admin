namespace FC.Codeflix.Catalog.Api.Models.Responses;

public record ApiResponseListMeta(int CurrentPage, int PerPage, int Total)
{
    public int CurrentPage { get; set; } = CurrentPage;
    public int PerPage { get; set; } = PerPage;
    public int Total { get; set; } = Total;
}
