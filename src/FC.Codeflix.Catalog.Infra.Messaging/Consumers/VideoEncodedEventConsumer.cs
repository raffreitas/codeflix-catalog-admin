using System.Text;
using System.Text.Json;

using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Videos.UpdateMediaStatus;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Infra.Messaging.Configuration;
using FC.Codeflix.Catalog.Infra.Messaging.DTOs;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FC.Codeflix.Catalog.Infra.Messaging.Consumers;

public sealed class VideoEncodedEventConsumer(
    IServiceProvider serviceProvider,
    ILogger<VideoEncodedEventConsumer> logger,
    IOptions<RabbitMqConfiguration> configuration,
    IChannel channel
) : BackgroundService
{
    private readonly string _queue = configuration.Value.VideoEncodedQueue!;

    private readonly JsonSerializerOptions _jsonOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += OnMessageReceived;
        await channel.BasicConsumeAsync(_queue, false, consumer, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(10_000, stoppingToken);
        }

        logger.LogWarning("Disposing Channel");
        await channel.DisposeAsync();
    }

    private async Task OnMessageReceived(object? sender, BasicDeliverEventArgs eventArgs)
    {
        string messageString = string.Empty;
        try
        {
            using var scope = serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var rawMessage = eventArgs.Body.ToArray();
            messageString = Encoding.UTF8.GetString(rawMessage);
            logger.LogDebug(messageString);
            var message = JsonSerializer.Deserialize<VideoEncodedMessageDTO>(messageString, _jsonOptions);
            var input = GetUpdateMediaStatusInput(message!);
            await mediator.Send(input, CancellationToken.None);
            channel.BasicAck(eventArgs.DeliveryTag, false);
        }
        catch (Exception ex)
            when (ex is EntityValidationException or NotFoundException)
        {
            logger.LogError(ex,
                "There was a business error in the message processing: {DeliveryTag}, {Message}",
                eventArgs.DeliveryTag, messageString);
            channel.BasicNack(eventArgs.DeliveryTag, false, false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "There was a unexpected error in the message processing: {DeliveryTag}, {Message}",
                eventArgs.DeliveryTag, messageString);
            channel.BasicNack(eventArgs.DeliveryTag, false, true);
        }
    }

    private static UpdateMediaStatusInput GetUpdateMediaStatusInput(VideoEncodedMessageDTO message)
    {
        if (message.Video is not null)
            return new UpdateMediaStatusInput(
                Guid.Parse(message.Video.ResourceId),
                MediaStatus.Completed,
                message.Video.FullEncodedVideoFilePath
            );

        return new UpdateMediaStatusInput(
            Guid.Parse(message.Message!.ResourceId),
            MediaStatus.Error,
            ErrorMessage: message.Error!
        );
    }
}