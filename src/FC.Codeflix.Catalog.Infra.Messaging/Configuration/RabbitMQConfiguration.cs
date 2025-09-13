namespace FC.Codeflix.Catalog.Infra.Messaging.Configuration;

public sealed record RabbitMqConfiguration
{
    public const string ConfigurationSection = "RabbitMQ";
    public required string Hostname { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string Exchange { get; init; }
};