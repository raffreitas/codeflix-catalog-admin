using System.Text.Json;

using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Infra.Messaging.Configuration;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;

namespace FC.Codeflix.Catalog.Infra.Messaging.Producer;

public sealed class RabbitMqProducer(
    IModel channel,
    IOptions<RabbitMqConfiguration> options
) : IMessageProducer
{
    private readonly string _exchange = options.Value.Exchange;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    public Task SendMessageAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        var routingKey = EventsMapping.GetRoutingKey<T>();
        var @event = JsonSerializer.SerializeToUtf8Bytes(message, _jsonSerializerOptions);

        channel.BasicPublish(
            exchange: _exchange,
            routingKey: routingKey,
            body: @event
        );

        channel.WaitForConfirmsOrDie();

        return Task.CompletedTask;
    }
}