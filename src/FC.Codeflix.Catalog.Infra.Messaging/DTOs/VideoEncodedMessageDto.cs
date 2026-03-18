namespace FC.Codeflix.Catalog.Infra.Messaging.DTOs;

public sealed class VideoEncodedMessageDto
{
    public VideoEncodedMetadataDto? Video { get; init; }
    public VideoEncodedMetadataDto? Message { get; init; }
    public string? Error { get; init; }
}

public sealed class VideoEncodedMetadataDto
{
    public string? EncodedVideoFolder { get; init; }
    public string ResourceId { get; init; } = null!;
    public string FilePath { get; init; } = null!;

    public string FullEncodedVideoFilePath => $"{EncodedVideoFolder}/{FilePath}";
}