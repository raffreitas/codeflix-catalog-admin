using FC.Codeflix.Catalog.Application.UseCases.Videos.DeleteVideo;
using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;

namespace FC.Codeflix.Catalog.UnitTests.Application.Videos.DeleteVideo;

public sealed class DeleteVideoTestFixture : VideoTestFixtureBase
{
    public DeleteVideoInput GetValidInput(Guid? id = null) => new(id ?? Guid.NewGuid());
}