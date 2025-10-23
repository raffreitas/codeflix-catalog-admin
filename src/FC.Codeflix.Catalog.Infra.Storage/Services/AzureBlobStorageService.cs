using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Infra.Storage.Settings;

using Microsoft.Extensions.Options;

namespace FC.Codeflix.Catalog.Infra.Storage.Services;

public sealed class AzureBlobStorageService(
    BlobServiceClient blobServiceClient,
    IOptions<StorageSettings> options
) : IStorageService
{
    private readonly StorageSettings _settings = options.Value;

    public async Task Delete(string filePath, CancellationToken cancellationToken)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(_settings.BucketName);
        var blobClient = containerClient.GetBlobClient(filePath);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<string> Upload(string fileName, Stream fileStream, string contentType,
        CancellationToken cancellationToken)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(_settings.BucketName);
        var blobClient = containerClient.GetBlobClient(fileName);

        var blobHttpHeaders = new BlobHttpHeaders { ContentType = contentType };

        await blobClient.UploadAsync(
            fileStream,
            new BlobUploadOptions { HttpHeaders = blobHttpHeaders },
            cancellationToken);

        return fileName;
    }
}