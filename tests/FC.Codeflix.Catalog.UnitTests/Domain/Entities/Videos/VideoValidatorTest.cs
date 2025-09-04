using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Validations;
using FC.Codeflix.Catalog.Domain.Validators;

using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.Videos;

public class VideoValidatorTest(VideoTestFixture fixture) : IClassFixture<VideoTestFixture>
{
    [Fact(DisplayName = nameof(ReturnsValidWhenVideoIsValid))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsValidWhenVideoIsValid()
    {
        var validVideo = fixture.GetValidVideo();
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(validVideo, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeFalse();
        notificationValidationHandler.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(ReturnsErrorWhenTitleIsTooLong))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsErrorWhenTitleIsTooLong()
    {
        var invalid = new Video(
            fixture.GetTooLongTitle(),
            fixture.GetValidDescription(),
            fixture.GetValidYearLaunched(),
            fixture.GetValidDuration(),
            fixture.GetRandomBoolean(),
            fixture.GetRandomBoolean()
        );
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(invalid, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(1);
        notificationValidationHandler.Errors.First()
            .Message
            .Should()
            .Be("'Title' should be less or equal 255 characters long");
    }

    [Fact(DisplayName = nameof(ReturnsErrorWhenTitleIsEmpty))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsErrorWhenTitleIsEmpty()
    {
        var invalid = new Video(
            string.Empty,
            fixture.GetValidDescription(),
            fixture.GetValidYearLaunched(),
            fixture.GetValidDuration(),
            fixture.GetRandomBoolean(),
            fixture.GetRandomBoolean()
        );
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(invalid, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(1);
        notificationValidationHandler.Errors.First()
            .Message
            .Should()
            .Be("'Title' is required");
    }

    [Fact(DisplayName = nameof(ReturnsErrorWhenDescriptionIsEmpty))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsErrorWhenDescriptionIsEmpty()
    {
        var invalid = new Video(
            fixture.GetValidTitle(),
            string.Empty,
            fixture.GetValidYearLaunched(),
            fixture.GetValidDuration(),
            fixture.GetRandomBoolean(),
            fixture.GetRandomBoolean()
        );
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(invalid, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(1);
        notificationValidationHandler.Errors.First()
            .Message
            .Should()
            .Be("'Description' is required");
    }

    [Fact(DisplayName = nameof(ReturnsErrorWhenDescriptionIsTooLong))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsErrorWhenDescriptionIsTooLong()
    {
        var invalid = new Video(
            fixture.GetValidTitle(),
            fixture.GetTooLongDescription(),
            fixture.GetValidYearLaunched(),
            fixture.GetValidDuration(),
            fixture.GetRandomBoolean(),
            fixture.GetRandomBoolean()
        );
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(invalid, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(1);
        notificationValidationHandler.Errors.First()
            .Message
            .Should()
            .Be("'Description' should be less or equal 4000 characters long");
    }
}