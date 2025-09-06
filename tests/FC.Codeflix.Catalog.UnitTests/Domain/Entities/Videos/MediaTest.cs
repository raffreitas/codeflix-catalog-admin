using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Enum;

using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.Videos;

[Trait("Domain", "Media - Entities")]
public sealed class MediaTest(VideoTestFixture fixture) : IClassFixture<VideoTestFixture>
{
    [Fact(DisplayName = nameof(Instantiate))]
    public void Instantiate()
    {
        var expectedFilePath = fixture.GetValidMediaPath();

        var media = new Media(expectedFilePath);

        media.Should().NotBeNull();
        media.Id.Should().NotBe(Guid.Empty);
        media.FilePath.Should().Be(expectedFilePath);
        media.Status.Should().Be(MediaStatus.Pending);
    }

    [Fact(DisplayName = nameof(UpdateAsSentToEncode))]
    public void UpdateAsSentToEncode()
    {
        var media = fixture.GetValidMedia();

        media.UpdateSentToEncode();

        media.Status.Should().Be(MediaStatus.Processing);
    }

    [Fact(DisplayName = nameof(UpdateAsEncoded))]
    public void UpdateAsEncoded()
    {
        var media = fixture.GetValidMedia();
        var encodedExpectedPath = fixture.GetValidMediaPath();
        media.UpdateSentToEncode();

        media.UpdateAsEncoded(encodedExpectedPath);

        media.Status.Should().Be(MediaStatus.Completed);
        media.EncodedPath.Should().Be(encodedExpectedPath);
    }
}