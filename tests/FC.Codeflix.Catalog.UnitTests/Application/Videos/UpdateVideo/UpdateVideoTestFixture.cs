using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;
using FC.Codeflix.Catalog.Application.UseCases.Videos.UpdateVideo;
using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;

namespace FC.Codeflix.Catalog.UnitTests.Application.Videos.UpdateVideo;

public sealed class UpdateVideoTestFixture : VideoTestFixtureBase
{
    public UpdateVideoInput CreateValidInput(
        Guid videoId,
        List<Guid>? genreIds = null,
        List<Guid>? categoryIds = null,
        List<Guid>? castMemberIds = null,
        FileInput? banner = null,
        FileInput? thumb = null,
        FileInput? thumbHalf = null
    ) => new(
        videoId,
        GetValidTitle(),
        GetValidDescription(),
        GetValidYearLaunched(),
        GetRandomBoolean(),
        GetRandomBoolean(),
        GetValidDuration(),
        GetRandomRating(),
        genreIds,
        categoryIds,
        castMemberIds,
        banner,
        thumb,
        thumbHalf
    );
}