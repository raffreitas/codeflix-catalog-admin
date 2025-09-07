namespace FC.Codeflix.Catalog.Infra.Messaging.Configuration;

public sealed record RabbitMqConfiguration
{
    public const string ConfigurationSection = "RabbitMQ";
    public required string HostName { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }
    public required string Exchange { get; init; }
};