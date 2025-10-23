namespace FC.Codeflix.Catalog.Infra.Storage.Settings;

public sealed record StorageSettings
{
    public const string ConfigurationSection = "Storage";
    public required string BucketName { get; init; }
    public required string ConnectionString { get; init; }
}