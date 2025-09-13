using FC.Codeflix.Catalog.Application;
using FC.Codeflix.Catalog.Application.EventHandlers;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Categories.CreateCategory;
using FC.Codeflix.Catalog.Domain.Events;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Infra.Messaging.Configuration;
using FC.Codeflix.Catalog.Infra.Messaging.Producer;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class UseCasesConfiguration
{
    public static IServiceCollection AddUseCases(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<CreateCategoryUseCase>());
        services.AddRepositories();
        services.AddDomainEvents(configuration);
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<ICastMemberRepository, CastMemberRepository>();
        services.AddScoped<IVideoRepository, VideoRepository>();
        return services;
    }

    private static IServiceCollection AddDomainEvents(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDomainEventPublisher, DomainEventPublisher>();
        services.AddTransient<IDomainEventHandler<VideoUploadedEvent>, SentToEncoderEventHandler>();

        services.Configure<RabbitMqConfiguration>(configuration.GetSection(RabbitMqConfiguration.ConfigurationSection));

        services.AddSingleton(sp =>
        {
            RabbitMqConfiguration config = sp.GetRequiredService<IOptions<RabbitMqConfiguration>>().Value;
            var factory = new ConnectionFactory
            {
                HostName = config.Hostname, UserName = config.Username, Password = config.Password
            };
            return factory.CreateConnection();
        });

        services.AddSingleton<ChannelManager>();

        services.AddTransient<IMessageProducer>(sp =>
        {
            var channelManager = sp.GetRequiredService<ChannelManager>();
            var config = sp.GetRequiredService<IOptions<RabbitMqConfiguration>>();
            return new RabbitMqProducer(channelManager.GetChannel()!, config);
        });

        return services;
    }
}