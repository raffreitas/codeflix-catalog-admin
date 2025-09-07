using Amazon.S3;
using Amazon.S3.Model;

using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Infra.Storage.Options;

using Microsoft.Extensions.Options;

namespace FC.Codeflix.Catalog.Infra.Storage.Services;

public sealed class AmazonS3StorageService(
    IAmazonS3 s3Client,
    IOptions<StorageServiceConfiguration> options
) : IStorageService
{
    private readonly StorageServiceConfiguration _configuration = options.Value;

    public async Task Delete(string filePath, CancellationToken cancellationToken)
        => await s3Client.DeleteObjectAsync(_configuration.BucketName, filePath, cancellationToken);

    public async Task<string> Upload(string fileName, Stream fileStream, string contentType,
        CancellationToken cancellationToken)
    {
        await s3Client.PutObjectAsync(
            new PutObjectRequest
            {
                ContentType = contentType,
                BucketName = _configuration.BucketName,
                Key = fileName,
                InputStream = fileStream
            }, cancellationToken);

        return fileName;
    }
}