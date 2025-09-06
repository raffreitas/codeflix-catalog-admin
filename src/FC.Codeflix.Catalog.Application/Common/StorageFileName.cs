namespace FC.Codeflix.Catalog.Application.Common;

public static class StorageFileName
{
    public static string Create(Guid exampleId, string propertyName, string exampleExtension)
        => $"{exampleId}-{propertyName.ToLower()}.{exampleExtension.Replace(".", string.Empty)}";
}