using System.Text.Json;

using Codeflix.Catalog.Application.Interfaces;
using Codeflix.Catalog.Infra.Messaging.Configuration;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;

namespace Codeflix.Catalog.Infra.Messaging.Producer;

public sealed class RabbitMqProducer(
    ChannelManager channelManager,
    IOptions<RabbitMqConfiguration> options
) : IMessageProducer
{
    private readonly string _exchange = options.Value.Exchange;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public async Task SendMessageAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        var routingKey = EventsMapping.GetRoutingKey<T>();
        var @event = JsonSerializer.SerializeToUtf8Bytes(message, _jsonSerializerOptions);
        var channel = await channelManager.GetChannelAsync(cancellationToken);

        using var publishLock = await channelManager.AcquirePublishLockAsync(cancellationToken);

        await channel.BasicPublishAsync(
            exchange: _exchange,
            routingKey: routingKey,
            mandatory: false,
            body: @event,
            cancellationToken: cancellationToken
        );
    }
}