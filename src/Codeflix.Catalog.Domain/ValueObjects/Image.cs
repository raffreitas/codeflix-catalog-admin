using Codeflix.Catalog.Domain.SeedWork;

namespace Codeflix.Catalog.Domain.ValueObjects;

public sealed class Image(string path) : ValueObject
{
    public string Path { get; } = path;

    public override bool Equals(ValueObject? other)
        => other is Image otherImage
           && otherImage.Path == Path;

    protected override int GetCustomHashCode()
        => HashCode.Combine(Path);
}