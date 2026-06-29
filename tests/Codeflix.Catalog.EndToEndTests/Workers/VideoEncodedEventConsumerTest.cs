using Codeflix.Catalog.EndToEndTests.Api.Videos.Common;

using Codeflix.Catalog.Domain.Enum;
using Codeflix.Catalog.Infra.Messaging.DTOs;

using FluentAssertions;

namespace Codeflix.Catalog.EndToEndTests.Workers;

[Collection(nameof(VideoBaseFixture))]
[Trait("E2E/API", "Video Encoded - Event Handler")]
public sealed class VideoEncodedEventConsumerTest(VideoBaseFixture fixture) : IDisposable
{
    [Fact(DisplayName = nameof(EncodingSucceededEventReceived))]
    public async Task EncodingSucceededEventReceived()
    {
        var exampleVideos = fixture.GetVideoCollection(5);
        await fixture.VideoPersistence.InsertList(exampleVideos);
        var video = exampleVideos[2];
        var encodedFilePath = fixture.GetValidMediaPath();
        var exampleEvent = new VideoEncodedMessageDto
        {
            Video = new VideoEncodedMetadataDto
            {
                EncodedVideoFolder = encodedFilePath,
                FilePath = video.Media!.FilePath,
                ResourceId = video.Id.ToString()
            }
        };

        fixture.PublishMessageToRabbitMQ(exampleEvent);

        await Task.Delay(800);
        var videoFromDB = await fixture.VideoPersistence.GetById(video.Id);
        videoFromDB.Should().NotBeNull();
        videoFromDB.Media!.Status.Should().Be(MediaStatus.Completed);
        videoFromDB.Media!.EncodedPath.Should().Be(exampleEvent.Video.FullEncodedVideoFilePath);
        (object? @event, uint count) = fixture.ReadMessageFromRabbitMQ<object>();
        @event.Should().BeNull();
        count.Should().Be(0);
    }

    [Fact(DisplayName = nameof(EncodingFailedEventReceived))]
    public async Task EncodingFailedEventReceived()
    {
        var exampleVideos = fixture.GetVideoCollection(5);
        await fixture.VideoPersistence.InsertList(exampleVideos);
        var video = exampleVideos[2];
        var exampleEvent = new VideoEncodedMessageDto
        {
            Message = new VideoEncodedMetadataDto
            {
                FilePath = video.Media!.FilePath, ResourceId = video.Id.ToString()
            },
            Error = "There was an error on processing the video."
        };

        fixture.PublishMessageToRabbitMQ(exampleEvent);

        await Task.Delay(800);
        var videoFromDB = await fixture.VideoPersistence.GetById(video.Id);
        videoFromDB.Should().NotBeNull();
        videoFromDB.Media!.Status.Should().Be(MediaStatus.Error);
        videoFromDB.Media!.FilePath.Should().NotBeNull();
        (object? @event, uint count) = fixture.ReadMessageFromRabbitMQ<object>();
        @event.Should().BeNull();
        count.Should().Be(0);
    }

    [Fact(DisplayName = nameof(InvalidMessageEventReceived))]
    public async Task InvalidMessageEventReceived()
    {
        var exampleVideos = fixture.GetVideoCollection(5);
        await fixture.VideoPersistence.InsertList(exampleVideos);
        var exampleEvent = new VideoEncodedMessageDto
        {
            Message = new VideoEncodedMetadataDto
            {
                FilePath = fixture.GetValidMediaPath(), ResourceId = Guid.NewGuid().ToString()
            },
            Error = "There was an error on processing the video."
        };

        fixture.PublishMessageToRabbitMQ(exampleEvent);

        await Task.Delay(800);
        (object? @event, uint count) = fixture.ReadMessageFromRabbitMQ<object>();
        @event.Should().BeNull();
        count.Should().Be(0);
    }

    public void Dispose()
    {
        fixture.CleanPersistence();
        fixture.PurgeRabbitMQQueues();
    }
}