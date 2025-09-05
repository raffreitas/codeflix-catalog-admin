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

        validVideo.Media.Should().NotBeNull();
        validVideo.Media.FilePath.Should().Be(validMediaPath);
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
}