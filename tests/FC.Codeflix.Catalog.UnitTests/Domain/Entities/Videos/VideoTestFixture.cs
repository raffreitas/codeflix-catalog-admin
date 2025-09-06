using System.Text;

using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;
using FC.Codeflix.Catalog.Application.UseCases.Videos.CreateVideo;
using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.Videos;

public sealed class VideoTestFixture : VideoTestFixtureBase
{
    public CreateVideoInput CreateValidCreateVideoInput(
        List<Guid>? categoriesIds = null,
        List<Guid>? genresIds = null,
        List<Guid>? castMembersIds = null,
        FileInput? thumb = null,
        FileInput? banner = null,
        FileInput? thumbHalf = null
    ) => new(
        GetValidTitle(),
        GetValidDescription(),
        GetValidYearLaunched(),
        GetValidDuration(),
        GetRandomBoolean(),
        GetRandomBoolean(),
        GetRandomRating(),
        categoriesIds,
        genresIds,
        castMembersIds,
        thumb,
        banner,
        thumbHalf
    );

    public CreateVideoInput CreateValidCreateVideoInputWithAllImages() => new(
        GetValidTitle(),
        GetValidDescription(),
        GetValidYearLaunched(),
        GetValidDuration(),
        GetRandomBoolean(),
        GetRandomBoolean(),
        GetRandomRating(),
        null,
        null,
        null,
        GetValidImageFileInput(),
        GetValidImageFileInput(),
        GetValidImageFileInput()
    );
}