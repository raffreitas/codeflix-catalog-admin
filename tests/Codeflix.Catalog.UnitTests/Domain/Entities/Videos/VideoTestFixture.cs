using Codeflix.Catalog.Application.UseCases.Videos.Common;
using Codeflix.Catalog.Application.UseCases.Videos.CreateVideo;
using Codeflix.Catalog.UnitTests.Common.Fixtures;

namespace Codeflix.Catalog.UnitTests.Domain.Entities.Videos;

public sealed class VideoTestFixture : VideoTestFixtureBase
{
    public CreateVideoInput CreateValidCreateVideoInput(
        List<Guid>? categoriesIds = null,
        List<Guid>? genresIds = null,
        List<Guid>? castMembersIds = null,
        FileInput? thumb = null,
        FileInput? banner = null,
        FileInput? thumbHalf = null,
        FileInput? media = null,
        FileInput? trailer = null
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
        thumbHalf,
        media,
        trailer
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

    public CreateVideoInput CreateValidInputWithAllMedias() => new(
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
        null,
        null,
        null,
        GetValidMediaFileInput(),
        GetValidMediaFileInput()
    );
}