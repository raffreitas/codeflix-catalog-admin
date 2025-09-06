namespace FC.Codeflix.Catalog.Application.UseCases.Videos.Common;

public sealed record FileInput(string Extension, Stream FileStream, string ContentType);