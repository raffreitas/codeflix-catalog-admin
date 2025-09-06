using FC.Codeflix.Catalog.Application.UseCases.Videos.UploadMedias;
using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;

namespace FC.Codeflix.Catalog.UnitTests.Application.Videos.UploadMedias;

public sealed class UploadMediasTestFixture : VideoTestFixtureBase
{
    public UploadMediasInput GetValidInput() => new(
        Guid.NewGuid(),
        GetValidMediaFileInput(),
        GetValidMediaFileInput()
    );
}