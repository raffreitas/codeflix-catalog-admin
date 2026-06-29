using Codeflix.Catalog.Application.Interfaces;
using Codeflix.Catalog.Infra.Messaging.Configuration;
using Codeflix.Catalog.Infra.Messaging.Consumers;
using Codeflix.Catalog.Infra.Messaging.Producer;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;

namespace Codeflix.Catalog.Api.Configurations;

public static class MessagingConfiguration
{
    private sealed record ProducerConnection(IConnection Connection) : IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            if (Connection.IsOpen)
                await Connection.CloseAsync();

            await Connection.DisposeAsync();
        }
    }

    private sealed record ConsumerConnection(IConnection Connection) : IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            if (Connection.IsOpen)
                await Connection.CloseAsync();

            await Connection.DisposeAsync();
        }
    }

    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqConfiguration>(configuration.GetSection(RabbitMqConfiguration.ConfigurationSection));

        services.AddSingleton(sp =>
        {
            RabbitMqConfiguration config = sp.GetRequiredService<IOptions<RabbitMqConfiguration>>().Value;
            return new ConnectionFactory
            {
                HostName = config.Hostname,
                UserName = config.Username,
                Password = config.Password,
                Port = config.Port
            };
        });

        services.AddSingleton<ProducerConnection>(sp =>
        {
            var factory = sp.GetRequiredService<ConnectionFactory>();
            var connection = factory.CreateConnectionAsync("codeflix-catalog-producer").GetAwaiter().GetResult();
            return new ProducerConnection(connection);
        });

        services.AddSingleton<ConsumerConnection>(sp =>
        {
            var factory = sp.GetRequiredService<ConnectionFactory>();
            var connection = factory.CreateConnectionAsync("codeflix-catalog-consumer").GetAwaiter().GetResult();
            return new ConsumerConnection(connection);
        });

        services.AddSingleton(sp => new ChannelManager(sp.GetRequiredService<ProducerConnection>().Connection));

        return services;
    }

    public static IServiceCollection AddMessageProducer(this IServiceCollection services)
    {
        services.AddTransient<IMessageProducer>(sp =>
        {
            var channelManager = sp.GetRequiredService<ChannelManager>();
            var config = sp.GetRequiredService<IOptions<RabbitMqConfiguration>>();
            return new RabbitMqProducer(channelManager, config);
        });
        return services;
    }

    public static IServiceCollection AddMessageConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService(sp =>
        {
            var cfg = sp.GetRequiredService<IOptions<RabbitMqConfiguration>>();
            var connection = sp.GetRequiredService<ConsumerConnection>();
            var logger = sp.GetRequiredService<ILogger<VideoEncodedEventConsumer>>();
            var channel = connection.Connection.CreateChannelAsync().GetAwaiter().GetResult();
            return new VideoEncodedEventConsumer(sp, logger, cfg, channel);
        });

        return services;
    }
}