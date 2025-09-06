using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.SeedWork;

namespace FC.Codeflix.Catalog.Domain.Entities;

public sealed class Media : Entity
{
    public string FilePath { get; private set; }
    public string? EncodedPath { get; private set; }
    public MediaStatus Status { get; private set; }

    public Media(string filePath)
    {
        FilePath = filePath;
        Status = MediaStatus.Pending;
    }

    public void UpdateSentToEncode() => Status = MediaStatus.Processing;

    public void UpdateAsEncoded(string encodedFilePath)
    {
        EncodedPath = encodedFilePath;
        Status = MediaStatus.Completed;
    }
}