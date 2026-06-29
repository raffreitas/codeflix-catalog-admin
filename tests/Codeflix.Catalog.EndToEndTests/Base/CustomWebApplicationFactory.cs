using Codeflix.Catalog.Application.Interfaces;
using Codeflix.Catalog.Infra.Data.EF;
using Codeflix.Catalog.Infra.Messaging.Configuration;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Moq;

using RabbitMQ.Client;

namespace Codeflix.Catalog.EndToEndTests.Base;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    private const string VideoCreatedRoutingKey = "video.created";
    public string VideoEncodedRoutingKey => "video.encoded";
    public string VideoCreatedQueue => "video.created.queue";

    public Mock<IStorageService>? StorageClient { get; private set; }
    public IChannel? RabbitMQChannel { get; private set; }
    public RabbitMqConfiguration? RabbitMQConfiguration { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("EndToEndTest");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IStorageService>();
            services.AddScoped(_ =>
            {
                StorageClient = new Mock<IStorageService>();
                StorageClient.Setup(x => x.Upload(
                    It.IsAny<string>(),
                    It.IsAny<Stream>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>())
                ).ReturnsAsync((string fileName, Stream _, string _, CancellationToken _) => fileName);
                return StorageClient.Object;
            });

            // Build a service provider to get required services for test setup.
            // Do NOT dispose this serviceProvider here: disposing it would close the RabbitMQ connection
            // and invalidate the channel we keep for setup/teardown.
            var serviceProvider = services.BuildServiceProvider();
            var dbContext = serviceProvider.GetRequiredService<CodeflixCatalogDbContext>();
            RabbitMQChannel = serviceProvider.GetRequiredService<ChannelManager>()
                .GetChannelAsync()
                .GetAwaiter()
                .GetResult();
            RabbitMQConfiguration = serviceProvider.GetRequiredService<IOptions<RabbitMqConfiguration>>().Value;
            SetupRabbitMq();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            // Note: serviceProvider is intentionally not disposed here to keep the RabbitMQ connection open
            // for the lifetime of the test factory.
        });

        base.ConfigureWebHost(builder);
    }


    public void SetupRabbitMq()
    {
        var channel = RabbitMQChannel;
        var exchange = RabbitMQConfiguration.Exchange;
        channel!.ExchangeDeclareAsync(exchange, "direct", true, false, null)
            .GetAwaiter()
            .GetResult();
        channel.QueueDeclareAsync(VideoCreatedQueue, true, false, false, null)
            .GetAwaiter()
            .GetResult();
        channel.QueueBindAsync(VideoCreatedQueue, exchange, VideoCreatedRoutingKey, null)
            .GetAwaiter()
            .GetResult();
        channel.QueueDeclareAsync(RabbitMQConfiguration!.VideoEncodedQueue, true, false, false, null)
            .GetAwaiter()
            .GetResult();
        channel.QueueBindAsync(RabbitMQConfiguration.VideoEncodedQueue, exchange, VideoEncodedRoutingKey, null)
            .GetAwaiter()
            .GetResult();
    }

    private void TearDownRabbitMq()
    {
        var channel = RabbitMQChannel;
        var exchange = RabbitMQConfiguration?.Exchange;
        if (channel == null || exchange == null)
            return;

        channel.QueueUnbindAsync(VideoCreatedQueue, exchange, VideoCreatedRoutingKey, null)
            .GetAwaiter()
            .GetResult();
        channel.QueueDeleteAsync(VideoCreatedQueue, false, false)
            .GetAwaiter()
            .GetResult();
        channel.QueueUnbindAsync(RabbitMQConfiguration!.VideoEncodedQueue, exchange, VideoEncodedRoutingKey, null)
            .GetAwaiter()
            .GetResult();
        channel.QueueDeleteAsync(RabbitMQConfiguration.VideoEncodedQueue, false, false)
            .GetAwaiter()
            .GetResult();
        channel.ExchangeDeleteAsync(exchange, false)
            .GetAwaiter()
            .GetResult();
    }

    public override ValueTask DisposeAsync()
    {
        TearDownRabbitMq();
        return base.DisposeAsync();
    }
}