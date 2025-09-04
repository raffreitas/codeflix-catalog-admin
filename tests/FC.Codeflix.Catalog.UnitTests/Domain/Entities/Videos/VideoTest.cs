using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Validations;

using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.Videos;

public sealed class VideoTest(VideoTestFixture fixture) : IClassFixture<VideoTestFixture>
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Video")]
    public void Instantiate()
    {
        var expectedTitle = fixture.GetValidTitle();
        var expectedDescription = fixture.GetValidDescription();
        var expectedYearLaunched = fixture.GetValidYearLaunched();
        var expectedDuration = fixture.GetValidDuration();
        var expectedOpened = fixture.GetRandomBoolean();
        var expectedPublished = fixture.GetRandomBoolean();

        var video = new Video(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedDuration,
            expectedOpened,
            expectedPublished
        );

        video.Should().NotBeNull();
        video.Title.Should().Be(expectedTitle);
        video.Description.Should().Be(expectedDescription);
        video.YearLaunched.Should().Be(expectedYearLaunched);
        video.Duration.Should().Be(expectedDuration);
        video.Opened.Should().Be(expectedOpened);
        video.Published.Should().Be(expectedPublished);
        video.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(Skip = "Old Way (Defer Validation)", DisplayName = nameof(InstantiateThrowsExceptionWhenNotValid))]
    [Trait("Domain", "Video")]
    public void InstantiateThrowsExceptionWhenNotValid()
    {
        var expectedTitle = string.Empty;
        var expectedDescription = fixture.GetTooLongDescription();
        var expectedYearLaunched = fixture.GetValidYearLaunched();
        var expectedDuration = fixture.GetValidDuration();
        var expectedOpened = fixture.GetRandomBoolean();
        var expectedPublished = fixture.GetRandomBoolean();

        var action = () => new Video(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedDuration,
            expectedOpened,
            expectedPublished
        );

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Validation Errors")
            .Which.Errors.Should().BeEquivalentTo(new List<ValidationError>
            {
                new("'Title' is required"), new("'Description' should be less or equal 4000 characters long")
            });
    }

    [Fact(DisplayName = nameof(ValidateWhenValidState))]
    [Trait("Domain", "Video")]
    public void ValidateWhenValidState()
    {
        var validVideo = fixture.GetValidVideo();
        var notificationHandler = new NotificationValidationHandler();

        validVideo.Validate(notificationHandler);

        notificationHandler.HasErrors().Should().BeFalse();
    }

    [Fact(DisplayName = nameof(ValidateWithErrorWhenInvalidState))]
    [Trait("Domain", "Video")]
    public void ValidateWithErrorWhenInvalidState()
    {
        var invalidVideo = new Video(
            fixture.GetTooLongTitle(),
            fixture.GetTooLongDescription(),
            fixture.GetValidYearLaunched(),
            fixture.GetValidDuration(),
            fixture.GetRandomBoolean(),
            fixture.GetRandomBoolean()
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
    [Trait("Domain", "Video")]
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
    [Trait("Domain", "Video")]
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
    [Trait("Domain", "Video")]
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
}