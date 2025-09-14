namespace FC.Codeflix.Catalog.Infra.Messaging.DTOs;

public sealed class VideoEncodedMessageDTO
{
    public VideoEncodedMetadataDTO? Video { get; set; }
    public VideoEncodedMetadataDTO? Message { get; set; }
    public string? Error { get; set; }
}

public sealed class VideoEncodedMetadataDTO
{
    public string? EncodedVideoFolder { get; set; }
    public string ResourceId { get; set; } = null!;
    public string FilePath { get; set; } = null!;

    public string FullEncodedVideoFilePath => $"{EncodedVideoFolder}/{FilePath}";
}