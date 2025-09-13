namespace FC.Codeflix.Catalog.Infra.Storage.Settings;

public sealed record StorageSettings
{
    public const string ConfigurationSection = "Storage";
    public required string BucketName { get; init; }
    public required string AccessKey { get; init; }
    public required string SecretKey { get; init; }
    public string? ServiceUrl { get; init; }
}