using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Validations;

using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.Videos;

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
    }

    [Fact(Skip = "Old Way (Defer Validation)", DisplayName = nameof(InstantiateThrowsExceptionWhenNotValid))]
    [Trait("Domain", "Video - Aggregates")]
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
    [Trait("Domain", "Video - Aggregates")]
    public void ValidateWhenValidState()
    {
        var validVideo = fixture.GetValidVideo();
        var notificationHandler = new NotificationValidationHandler();

        validVideo.Validate(notificationHandler);

        notificationHandler.HasErrors().Should().BeFalse();
    }

    [Fact(DisplayName = nameof(ValidateWithErrorWhenInvalidState))]
    [Trait("Domain", "Video - Aggregates")]
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
    [Trait("Domain", "Video - Aggregates")]
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
    [Trait("Domain", "Video - Aggregates")]
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
    [Trait("Domain", "Video - Aggregates")]
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
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateThumb()
    {
        var validVideo = fixture.GetValidVideo();
        var validImagePath = fixture.GetValidImagePath();

        validVideo.UpdateThumb(validImagePath);

        validVideo.Thumb.Should().NotBeNull();
        validVideo.Thumb.Path.Should().Be(validImagePath);
    }

    [Fact(DisplayName = nameof(UpdateThumbHalf))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateThumbHalf()
    {
        var validVideo = fixture.GetValidVideo();
        var validImagePath = fixture.GetValidImagePath();

        validVideo.UpdateThumbHalf(validImagePath);

        validVideo.ThumbHalf.Should().NotBeNull();
        validVideo.ThumbHalf.Path.Should().Be(validImagePath);
    }

    [Fact(DisplayName = nameof(UpdateBanner))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateBanner()
    {
        var validVideo = fixture.GetValidVideo();
        var validImagePath = fixture.GetValidImagePath();

        validVideo.UpdateBanner(validImagePath);

        validVideo.Banner.Should().NotBeNull();
        validVideo.Banner.Path.Should().Be(validImagePath);
    }
}