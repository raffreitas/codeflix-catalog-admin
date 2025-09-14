using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Messaging.Configuration;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Moq;

using RabbitMQ.Client;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    private const string VideoCreatedRoutingKey = "video.created";
    public string VideoEncodedRoutingKey => "video.encoded";
    public string VideoCreatedQueue => "video.created.queue";

    public Mock<IStorageService> StorageClient { get; private set; }
    public IModel RabbitMQChannel { get; private set; }
    public RabbitMqConfiguration RabbitMQConfiguration { get; private set; }

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

            using var serviceProvider = services.BuildServiceProvider();
            using var dbContext = serviceProvider.GetRequiredService<CodeflixCatalogDbContext>();
            RabbitMQChannel = serviceProvider.GetRequiredService<ChannelManager>().GetChannel()!;
            RabbitMQConfiguration = serviceProvider.GetRequiredService<IOptions<RabbitMqConfiguration>>().Value;
            SetupRabbitMq();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        });

        base.ConfigureWebHost(builder);
    }


    public void SetupRabbitMq()
    {
        var channel = RabbitMQChannel;
        var exchange = RabbitMQConfiguration.Exchange;
        channel.ExchangeDeclare(exchange, "direct", true, false, null);
        channel.QueueDeclare(VideoCreatedQueue, true, false, false, null);
        channel.QueueBind(VideoCreatedQueue, exchange, VideoCreatedRoutingKey, null);
        channel.QueueDeclare(RabbitMQConfiguration.VideoEncodedQueue, true, false, false, null);
        channel.QueueBind(RabbitMQConfiguration.VideoEncodedQueue, exchange, VideoEncodedRoutingKey, null);
    }

    private void TearDownRabbitMq()
    {
        var channel = RabbitMQChannel!;
        var exchange = RabbitMQConfiguration!.Exchange;
        channel.QueueUnbind(VideoCreatedQueue, exchange, VideoCreatedRoutingKey, null);
        channel.QueueDelete(VideoCreatedQueue, false, false);
        channel.QueueUnbind(RabbitMQConfiguration.VideoEncodedQueue, exchange, VideoEncodedRoutingKey, null);
        channel.QueueDelete(RabbitMQConfiguration.VideoEncodedQueue, false, false);
        channel.ExchangeDelete(exchange, false);
    }

    public override ValueTask DisposeAsync()
    {
        TearDownRabbitMq();
        return base.DisposeAsync();
    }
}