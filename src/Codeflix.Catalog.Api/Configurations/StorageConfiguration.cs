using Azure.Storage.Blobs;

using Codeflix.Catalog.Application.Interfaces;
using Codeflix.Catalog.Infra.Storage.Services;
using Codeflix.Catalog.Infra.Storage.Settings;

namespace Codeflix.Catalog.Api.Configurations;

public static class StorageConfiguration
{
    public static IServiceCollection AddStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<StorageSettings>(configuration.GetSection(StorageSettings.ConfigurationSection));

        var settings = configuration.GetSection(StorageSettings.ConfigurationSection).Get<StorageSettings>() ??
                       throw new InvalidOperationException("Storage settings not found");

        services.AddSingleton(_ => new BlobServiceClient(settings.ConnectionString));
        services.AddScoped<IStorageService, AzureBlobStorageService>();

        return services;
    }
}