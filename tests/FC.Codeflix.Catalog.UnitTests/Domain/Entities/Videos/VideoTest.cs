using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Validations;

using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.Videos;

[Trait("Domain", "Video - Aggregates")]
public sealed class VideoTest(VideoTestFixture fixture) : IClassFixture<VideoTestFixture>
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Video - Aggregates")]
    public void Instantiate()
    {
        var expectedTitle = fixture.GetValidTitle();
        var expectedDescription = fixture.GetValidDescription();
        var expectedYearLaunched = fixture.GetValidYearLaunched();
        var expectedDuration = fixture.GetValidDuration();
        var expectedOpened = fixture.GetRandomBoolean();
        var expectedPublished = fixture.GetRandomBoolean();
        var expectedRating = fixture.GetRandomRating();

        var video = new Video(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedDuration,
            expectedOpened,
            expectedPublished,
            expectedRating
        );

        video.Should().NotBeNull();
        video.Title.Should().Be(expectedTitle);
        video.Description.Should().Be(expectedDescription);
        video.YearLaunched.Should().Be(expectedYearLaunched);
        video.Duration.Should().Be(expectedDuration);
        video.Opened.Should().Be(expectedOpened);
        video.Published.Should().Be(expectedPublished);
        video.CreatedAt.Should().NotBeSameDateAs(default);
        video.Thumb.Should().BeNull();
        video.ThumbHalf.Should().BeNull();
        video.Banner.Should().BeNull();
        video.Media.Should().BeNull();
        video.Trailer.Should().BeNull();
    }

    [Fact(Skip = "Old Way (Defer Validation)", DisplayName = nameof(InstantiateThrowsExceptionWhenNotValid))]
    public void InstantiateThrowsExceptionWhenNotValid()
    {
        var expectedTitle = string.Empty;
        var expectedDescription = fixture.GetTooLongDescription();
        var expectedYearLaunched = fixture.GetValidYearLaunched();
        var expectedDuration = fixture.GetValidDuration();
        var expectedOpened = fixture.GetRandomBoolean();
        var expectedPublished = fixture.GetRandomBoolean();
        var expectedRating = fixture.GetRandomRating();

        var action = () => new Video(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedDuration,
            expectedOpened,
            expectedPublished,
            expectedRating
        );

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Validation Errors")
            .Which.Errors.Should().BeEquivalentTo(new List<ValidationError>
            {
                new("'Title' is required"), new("'Description' should be less or equal 4000 characters long")
            });
    }

    [Fact(DisplayName = nameof(ValidateWhenValidState))]
    public void ValidateWhenValidState()
    {
        var validVideo = fixture.GetValidVideo();
        var notificationHandler = new NotificationValidationHandler();

        validVideo.Validate(notificationHandler);

        notificationHandler.HasErrors().Should().BeFalse();
    }

    [Fact(DisplayName = nameof(ValidateWithErrorWhenInvalidState))]
    public void ValidateWithErrorWhenInvalidState()
    {
        var invalidVideo = new Video(
            fixture.GetTooLongTitle(),
            fixture.GetTooLongDescription(),
            fixture.GetValidYearLaunched(),
            fixture.GetValidDuration(),
            fixture.GetRandomBoolean(),
            fixture.GetRandomBoolean(),
            fixture.GetRandomRating()
        );

        var notificationHandler = new NotificationValidationHandler();

        invalidVideo.Validate(notificationHandler);

        notificationHandler.HasErrors().Should().BeTrue();
        notificationHandler.Errors.Should().BeEquivalentTo(new List<ValidationError>
        {
            new("'Title' should be less or equal 255 characters long"),
            new("'Description' should be less or equal 4000 characters long")
        });
    }

    [Fact(DisplayName = nameof(Update))]
    public void Update()
    {
        var expectedTitle = fixture.GetValidTitle();
        var expectedDescription = fixture.GetValidDescription();
        var expectedYearLaunched = fixture.GetValidYearLaunched();
        var expectedDuration = fixture.GetValidDuration();
        var expectedOpened = fixture.GetRandomBoolean();
        var expectedPublished = fixture.GetRandomBoolean();
        var video = fixture.GetValidVideo();

        video.Update(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedDuration,
            expectedOpened,
            expectedPublished
        );

        video.Title.Should().Be(expectedTitle);
        video.Description.Should().Be(expectedDescription);
        video.YearLaunched.Should().Be(expectedYearLaunched);
        video.Duration.Should().Be(expectedDuration);
        video.Opened.Should().Be(expectedOpened);
        video.Published.Should().Be(expectedPublished);
        video.CreatedAt.Should().NotBeSameDateAs(default);
    }


    [Fact(DisplayName = nameof(UpdateWithRating))]
    public void UpdateWithRating()
    {
        var expectedTitle = fixture.GetValidTitle();
        var expectedDescription = fixture.GetValidDescription();
        var expectedYearLaunched = fixture.GetValidYearLaunched();
        var expectedOpened = fixture.GetRandomBoolean();
        var expectedPublished = fixture.GetRandomBoolean();
        var expectedDuration = fixture.GetValidDuration();
        var expectedRating = fixture.GetRandomRating();
        var video = fixture.GetValidVideo();

        video.Update(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedDuration,
            expectedOpened,
            expectedPublished,
            expectedRating
        );

        video.Title.Should().Be(expectedTitle);
        video.Description.Should().Be(expectedDescription);
        video.YearLaunched.Should().Be(expectedYearLaunched);
        video.Opened.Should().Be(expectedOpened);
        video.Published.Should().Be(expectedPublished);
        video.Duration.Should().Be(expectedDuration);
        video.Rating.Should().Be(expectedRating);
    }

    [Fact(DisplayName = nameof(UpdateWithoutRatingDoesntChangeTheRating))]
    [Trait("Domain", "Video - Aggregate")]
    public void UpdateWithoutRatingDoesntChangeTheRating()
    {
        var video = fixture.GetValidVideo();
        var expectedRating = video.Rating;

        video.Update(
            fixture.GetValidTitle(),
            fixture.GetValidDescription(),
            fixture.GetValidYearLaunched(),
            fixture.GetValidDuration(),
            fixture.GetRandomBoolean(),
            fixture.GetRandomBoolean()
        );

        video.Rating.Should().Be(expectedRating);
    }

    [Fact(DisplayName = nameof(ValidateStillValidatingAfterUpdate))]
    public void ValidateStillValidatingAfterUpdate()
    {
        var expectedTitle = fixture.GetValidTitle();
        var expectedDescription = fixture.GetValidDescription();
        var expectedYearLaunched = fixture.GetValidYearLaunched();
        var expectedDuration = fixture.GetValidDuration();
        var expectedOpened = fixture.GetRandomBoolean();
        var expectedPublished = fixture.GetRandomBoolean();
        var video = fixture.GetValidVideo();

        video.Update(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedDuration,
            expectedOpened,
            expectedPublished
        );
        var notificationHandler = new NotificationValidationHandler();
        video.Validate(notificationHandler);

        notificationHandler.HasErrors().Should().BeFalse();
    }

    [Fact(DisplayName = nameof(ValidateStillValidatingAfterUpdateToInvalidState))]
    public void ValidateStillValidatingAfterUpdateToInvalidState()
    {
        var expectedTitle = fixture.GetTooLongTitle();
        var expectedDescription = fixture.GetTooLongDescription();
        var expectedYearLaunched = fixture.GetValidYearLaunched();
        var expectedDuration = fixture.GetValidDuration();
        var expectedOpened = fixture.GetRandomBoolean();
        var expectedPublished = fixture.GetRandomBoolean();
        var video = fixture.GetValidVideo();

        video.Update(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedDuration,
            expectedOpened,
            expectedPublished
        );
        var notificationHandler = new NotificationValidationHandler();
        video.Validate(notificationHandler);

        notificationHandler.HasErrors().Should().BeTrue();
        notificationHandler.Errors.Should().HaveCount(2);
    }


    [Fact(DisplayName = nameof(UpdateThumb))]
    public void UpdateThumb()
    {
        var validVideo = fixture.GetValidVideo();
        var validImagePath = fixture.GetValidImagePath();

        validVideo.UpdateThumb(validImagePath);

        validVideo.Thumb.Should().NotBeNull();
        validVideo.Thumb.Path.Should().Be(validImagePath);
    }

    [Fact(DisplayName = nameof(UpdateThumbHalf))]
    public void UpdateThumbHalf()
    {
        var validVideo = fixture.GetValidVideo();
        var validImagePath = fixture.GetValidImagePath();

        validVideo.UpdateThumbHalf(validImagePath);

        validVideo.ThumbHalf.Should().NotBeNull();
        validVideo.ThumbHalf.Path.Should().Be(validImagePath);
    }

    [Fact(DisplayName = nameof(UpdateBanner))]
    public void UpdateBanner()
    {
        var validVideo = fixture.GetValidVideo();
        var validImagePath = fixture.GetValidImagePath();

        validVideo.UpdateBanner(validImagePath);

        validVideo.Banner.Should().NotBeNull();
        validVideo.Banner.Path.Should().Be(validImagePath);
    }

    [Fact(DisplayName = nameof(UpdateMedia))]
    public void UpdateMedia()
    {
        var validVideo = fixture.GetValidVideo();
        var validMediaPath = fixture.GetValidMediaPath();

        validVideo.UpdateMedia(validMediaPath);

        validVideo.Media.Should().NotBeNull();
        validVideo.Media.FilePath.Should().Be(validMediaPath);
    }

    [Fact(DisplayName = nameof(UpdateTrailer))]
    public void UpdateTrailer()
    {
        var validVideo = fixture.GetValidVideo();
        var validMediaPath = fixture.GetValidMediaPath();

        validVideo.UpdateTrailer(validMediaPath);

        validVideo.Trailer.Should().NotBeNull();
        validVideo.Trailer.FilePath.Should().Be(validMediaPath);
    }

    [Fact(DisplayName = nameof(UpdateAsSentToEncode))]
    public void UpdateAsSentToEncode()
    {
        var validVideo = fixture.GetValidVideo();
        var validMediaPath = fixture.GetValidMediaPath();
        validVideo.UpdateMedia(validMediaPath);

        validVideo.UpdateSentToEncode();

        validVideo.Media.Should().NotBeNull();
        validVideo.Media.Status.Should().Be(MediaStatus.Processing);
    }

    [Fact(DisplayName = nameof(UpdateAsSentToEncodeThrowsWhenThereIsNoMedia))]
    public void UpdateAsSentToEncodeThrowsWhenThereIsNoMedia()
    {
        var validVideo = fixture.GetValidVideo();

        var act = () => validVideo.UpdateSentToEncode();

        act.Should().Throw<EntityValidationException>()
            .WithMessage("There is no Media");
    }

    [Fact(DisplayName = nameof(UpdateAsEncoded))]
    public void UpdateAsEncoded()
    {
        var validVideo = fixture.GetValidVideo();
        var validMediaPath = fixture.GetValidMediaPath();
        var validEncodedPath = fixture.GetValidMediaPath();
        validVideo.UpdateMedia(validMediaPath);

        validVideo.UpdateAsEncoded(validEncodedPath);

        validVideo.Media.Should().NotBeNull();
        validVideo.Media.Status.Should().Be(MediaStatus.Completed);
        validVideo.Media.EncodedPath.Should().Be(validEncodedPath);
    }

    [Fact(DisplayName = nameof(UpdateAsEncodedThrowsWhenThereIsNoMedia))]
    public void UpdateAsEncodedThrowsWhenThereIsNoMedia()
    {
        var validVideo = fixture.GetValidVideo();
        var validEncodedPath = fixture.GetValidMediaPath();

        var act = () => validVideo.UpdateAsEncoded(validEncodedPath);

        act.Should().Throw<EntityValidationException>()
            .WithMessage("There is no Media");
    }

    [Fact(DisplayName = nameof(AddCategory))]
    public void AddCategory()
    {
        var validVideo = fixture.GetValidVideo();
        var categoryIdExample = Guid.NewGuid();

        validVideo.AddCategory(categoryIdExample);

        validVideo.Categories.Should().HaveCount(1);
        validVideo.Categories[0].Should().Be(categoryIdExample);
    }

    [Fact(DisplayName = nameof(RemoveCategory))]
    public void RemoveCategory()
    {
        var validVideo = fixture.GetValidVideo();
        var categoryIdExample = Guid.NewGuid();
        var categoryIdExample2 = Guid.NewGuid();
        validVideo.AddCategory(categoryIdExample);
        validVideo.AddCategory(categoryIdExample2);

        validVideo.RemoveCategory(categoryIdExample2);

        validVideo.Categories.Should().HaveCount(1);
        validVideo.Categories[0].Should().Be(categoryIdExample);
    }

    [Fact(DisplayName = nameof(RemoveAllCategory))]
    public void RemoveAllCategory()
    {
        var validVideo = fixture.GetValidVideo();
        var categoryIdExample = Guid.NewGuid();
        var categoryIdExample2 = Guid.NewGuid();
        validVideo.AddCategory(categoryIdExample);
        validVideo.AddCategory(categoryIdExample2);

        validVideo.RemoveAllCategories();

        validVideo.Categories.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(AddGenre))]
    public void AddGenre()
    {
        var validVideo = fixture.GetValidVideo();
        var genreIdExample = Guid.NewGuid();

        validVideo.AddGenre(genreIdExample);

        validVideo.Genres.Should().HaveCount(1);
        validVideo.Genres[0].Should().Be(genreIdExample);
    }

    [Fact(DisplayName = nameof(RemoveGenre))]
    public void RemoveGenre()
    {
        var validVideo = fixture.GetValidVideo();
        var genreIdExample = Guid.NewGuid();
        var genreIdExample2 = Guid.NewGuid();
        validVideo.AddGenre(genreIdExample);
        validVideo.AddGenre(genreIdExample2);

        validVideo.RemoveGenre(genreIdExample2);

        validVideo.Genres.Should().HaveCount(1);
        validVideo.Genres[0].Should().Be(genreIdExample);
    }

    [Fact(DisplayName = nameof(RemoveAllGenres))]
    public void RemoveAllGenres()
    {
        var validVideo = fixture.GetValidVideo();
        var genreIdExample = Guid.NewGuid();
        var genreIdExample2 = Guid.NewGuid();
        validVideo.AddGenre(genreIdExample);
        validVideo.AddGenre(genreIdExample2);

        validVideo.RemoveAllGenres();

        validVideo.Genres.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(AddCastMember))]
    public void AddCastMember()
    {
        var validVideo = fixture.GetValidVideo();
        var castMemberExample = Guid.NewGuid();

        validVideo.AddCastMember(castMemberExample);

        validVideo.CastMembers.Should().HaveCount(1);
        validVideo.CastMembers[0].Should().Be(castMemberExample);
    }

    [Fact(DisplayName = nameof(RemoveCastMember))]
    public void RemoveCastMember()
    {
        var validVideo = fixture.GetValidVideo();
        var castMemberExample = Guid.NewGuid();
        var castMemberExample2 = Guid.NewGuid();
        validVideo.AddCastMember(castMemberExample);
        validVideo.AddCastMember(castMemberExample2);

        validVideo.RemoveCastMember(castMemberExample2);

        validVideo.CastMembers.Should().HaveCount(1);
        validVideo.CastMembers[0].Should().Be(castMemberExample);
    }

    [Fact(DisplayName = nameof(RemoveAllCastMembers))]
    public void RemoveAllCastMembers()
    {
        var validVideo = fixture.GetValidVideo();
        var castMemberExample = Guid.NewGuid();
        var castMemberExample2 = Guid.NewGuid();
        validVideo.AddCastMember(castMemberExample);
        validVideo.AddCastMember(castMemberExample2);

        validVideo.RemoveAllCastMembers();

        validVideo.CastMembers.Should().HaveCount(0);
    }
}