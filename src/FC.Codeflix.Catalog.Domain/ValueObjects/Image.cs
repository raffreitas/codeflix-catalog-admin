using FC.Codeflix.Catalog.Domain.SeedWork;

namespace FC.Codeflix.Catalog.Domain.ValueObjects;

public sealed class Image : ValueObject
{
    public string Path { get; }

    public Image(string path)
        => Path = path;

    public override bool Equals(ValueObject? other)
        => other is Image otherImage
           && otherImage.Path == Path;

    protected override int GetCustomHashCode()
        => HashCode.Combine(Path);
}