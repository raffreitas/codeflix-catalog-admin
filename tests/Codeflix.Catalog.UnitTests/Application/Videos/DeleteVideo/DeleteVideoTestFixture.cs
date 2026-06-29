using Codeflix.Catalog.Application.UseCases.Videos.DeleteVideo;
using Codeflix.Catalog.UnitTests.Common.Fixtures;

namespace Codeflix.Catalog.UnitTests.Application.Videos.DeleteVideo;

public sealed class DeleteVideoTestFixture : VideoTestFixtureBase
{
    public DeleteVideoInput GetValidInput(Guid? id = null) => new(id ?? Guid.NewGuid());
}