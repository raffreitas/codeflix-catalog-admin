using Codeflix.Catalog.Application;
using Codeflix.Catalog.Application.EventHandlers;
using Codeflix.Catalog.Application.Interfaces;
using Codeflix.Catalog.Application.UseCases.Categories.CreateCategory;
using Codeflix.Catalog.Domain.Events;
using Codeflix.Catalog.Domain.Repositories;
using Codeflix.Catalog.Domain.SeedWork;
using Codeflix.Catalog.Infra.Data.EF;
using Codeflix.Catalog.Infra.Data.EF.Repositories;
using Codeflix.Catalog.Infra.Messaging.Configuration;
using Codeflix.Catalog.Infra.Messaging.Producer;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;

namespace Codeflix.Catalog.Api.Configurations;

public static class UseCasesConfiguration
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<CreateCategoryUseCase>());
        services.AddRepositories();
        services.AddDomainEvents();
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

    private static IServiceCollection AddDomainEvents(this IServiceCollection services)
    {
        services.AddTransient<IDomainEventPublisher, DomainEventPublisher>();
        services.AddTransient<IDomainEventHandler<VideoUploadedEvent>, SentToEncoderEventHandler>();
        return services;
    }
}