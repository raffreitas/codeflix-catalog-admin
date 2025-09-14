using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Infra.Messaging.Configuration;
using FC.Codeflix.Catalog.Infra.Messaging.Consumers;
using FC.Codeflix.Catalog.Infra.Messaging.Producer;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class MessagingConfiguration
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqConfiguration>(configuration.GetSection(RabbitMqConfiguration.ConfigurationSection));

        services.AddSingleton(sp =>
        {
            RabbitMqConfiguration config = sp.GetRequiredService<IOptions<RabbitMqConfiguration>>().Value;
            var factory = new ConnectionFactory
            {
                HostName = config.Hostname,
                UserName = config.Username,
                Password = config.Password,
                Port = config.Port
            };
            return factory.CreateConnection();
        });

        services.AddSingleton<ChannelManager>();

        return services;
    }

    public static IServiceCollection AddMessageProducer(this IServiceCollection services)
    {
        services.AddTransient<IMessageProducer>(sp =>
        {
            var channelManager = sp.GetRequiredService<ChannelManager>();
            var config = sp.GetRequiredService<IOptions<RabbitMqConfiguration>>();
            return new RabbitMqProducer(channelManager.GetChannel()!, config);
        });
        return services;
    }

    public static IServiceCollection AddMessageConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService(sp =>
        {
            var cfg = sp.GetRequiredService<IOptions<RabbitMqConfiguration>>();
            var connection = sp.GetRequiredService<IConnection>();
            var logger = sp.GetRequiredService<ILogger<VideoEncodedEventConsumer>>();
            return new VideoEncodedEventConsumer(sp, logger, cfg, connection.CreateModel());
        });

        return services;
    }
}