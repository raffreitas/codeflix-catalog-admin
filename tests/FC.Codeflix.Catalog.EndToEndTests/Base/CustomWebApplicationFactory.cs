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
                return StorageClient.Object;
            });

            using var serviceProvider = services.BuildServiceProvider();
            using var dbContext = serviceProvider.GetRequiredService<CodeflixCatalogDbContext>();
            RabbitMQChannel = serviceProvider.GetRequiredService<ChannelManager>().GetChannel()!;
            RabbitMQConfiguration = serviceProvider.GetRequiredService<IOptions<RabbitMqConfiguration>>().Value;

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        });

        base.ConfigureWebHost(builder);
    }
}