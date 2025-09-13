using Amazon.S3;

using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Infra.Storage.Services;
using FC.Codeflix.Catalog.Infra.Storage.Settings;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class StorageConfiguration
{
    public static IServiceCollection AddStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration.Get<StorageSettings>() ??
                       throw new InvalidOperationException("Storage settings not found");

        var config = new AmazonS3Config();
        if (!string.IsNullOrWhiteSpace(settings.ServiceUrl))
        {
            config.ServiceURL = settings.ServiceUrl;
            config.ForcePathStyle = true;
        }

        services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(settings.AccessKey, settings.SecretKey, config));
        services.Configure<StorageSettings>(configuration.GetSection(StorageSettings.ConfigurationSection));
        services.AddScoped<IStorageService, AmazonS3StorageService>();
        return services;
    }
}