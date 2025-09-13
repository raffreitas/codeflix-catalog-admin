using FC.Codeflix.Catalog.Application.Common;

using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Common;

[Trait("Application", "StorageName - Common")]
public sealed class StorageFileNameTest
{
    [Fact]
    public void CreateStorageNameForFile()
    {
        var exampleId = Guid.NewGuid();
        const string exampleExtension = "mp4";
        const string propertyName = "Video";

        var name = StorageFileName.Create(exampleId, propertyName, exampleExtension);

        name.Should().Be($"{exampleId}/{propertyName.ToLower()}.{exampleExtension}");
    }
}