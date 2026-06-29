namespace Codeflix.Catalog.Api.Models.Responses;

public record ApiResponse<TData>(TData Data)
{
    public TData Data { get; set; } = Data;
}
