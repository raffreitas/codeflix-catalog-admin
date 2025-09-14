using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.EndToEndTests.Api.Videos.Common;
using FC.Codeflix.Catalog.Infra.Messaging.DTOs;

using FluentAssertions;

namespace FC.Codeflix.Catalog.EndToEndTests.Workers;

[Collection(nameof(VideoBaseFixture))]
[Trait("E2E/API", "Video Encoded - Event Handler")]
public sealed class VideoEncodedEventConsumerTest : IDisposable
{
    private readonly VideoBaseFixture _fixture;

    public VideoEncodedEventConsumerTest(VideoBaseFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = nameof(EncodingSucceededEventReceived))]
    public async Task EncodingSucceededEventReceived()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);
        var video = exampleVideos[2];
        var encodedFilePath = _fixture.GetValidMediaPath();
        var exampleEvent = new VideoEncodedMessageDTO
        {
            Video = new VideoEncodedMetadataDTO
            {
                EncodedVideoFolder = encodedFilePath,
                FilePath = video.Media!.FilePath,
                ResourceId = video.Id.ToString()
            }
        };

        _fixture.PublishMessageToRabbitMQ(exampleEvent);

        await Task.Delay(800);
        var videoFromDB = await _fixture.VideoPersistence.GetById(video.Id);
        videoFromDB.Should().NotBeNull();
        videoFromDB.Media!.Status.Should().Be(MediaStatus.Completed);
        videoFromDB.Media!.EncodedPath.Should().Be(exampleEvent.Video.FullEncodedVideoFilePath);
        (object? @event, uint count) = _fixture.ReadMessageFromRabbitMQ<object>();
        @event.Should().BeNull();
        count.Should().Be(0);
    }

    [Fact(DisplayName = nameof(EncodingFailedEventReceived))]
    public async Task EncodingFailedEventReceived()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);
        var video = exampleVideos[2];
        var exampleEvent = new VideoEncodedMessageDTO
        {
            Message = new VideoEncodedMetadataDTO
            {
                FilePath = video.Media!.FilePath, ResourceId = video.Id.ToString()
            },
            Error = "There was an error on processing the video."
        };

        _fixture.PublishMessageToRabbitMQ(exampleEvent);

        await Task.Delay(800);
        var videoFromDB = await _fixture.VideoPersistence.GetById(video.Id);
        videoFromDB.Should().NotBeNull();
        videoFromDB.Media!.Status.Should().Be(MediaStatus.Error);
        videoFromDB.Media!.FilePath.Should().NotBeNull();
        (object? @event, uint count) = _fixture.ReadMessageFromRabbitMQ<object>();
        @event.Should().BeNull();
        count.Should().Be(0);
    }

    [Fact(DisplayName = nameof(InvalidMessageEventReceived))]
    public async Task InvalidMessageEventReceived()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);
        var exampleEvent = new VideoEncodedMessageDTO
        {
            Message = new VideoEncodedMetadataDTO
            {
                FilePath = _fixture.GetValidMediaPath(), ResourceId = Guid.NewGuid().ToString()
            },
            Error = "There was an error on processing the video."
        };

        _fixture.PublishMessageToRabbitMQ(exampleEvent);

        await Task.Delay(800);
        (object? @event, uint count) = _fixture.ReadMessageFromRabbitMQ<object>();
        @event.Should().BeNull();
        count.Should().Be(0);
    }

    public void Dispose()
    {
        _fixture.CleanPersistence();
        _fixture.PurgeRabbitMQQueues();
    }
}