using System.Net;

using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Events;
using FC.Codeflix.Catalog.EndToEndTests.Api.Videos.Common;
using FC.Codeflix.Catalog.EndToEndTests.Extensions;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Videos.UploadMedias;

[Collection(nameof(VideoBaseFixture))]
[Trait("EndToEnd/Api", "Video/UploadMedias - Endpoints")]
public class UploadMediasApiTest(VideoBaseFixture fixture) : IDisposable
{
    [Fact(DisplayName = nameof(UploadBanner))]
    public async Task UploadBanner()
    {
        var exampleVideos = fixture.GetVideoCollection(5);
        await fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = exampleVideos[2].Id;
        var mediaType = "banner";
        var file = fixture.GetValidImageFileInput();
        var expectedFileName = StorageFileName.Create(videoId, nameof(Video.Banner), file.Extension);
        var expectedContent = file.FileStream.ToContentString();

        var (response, output) =
            await fixture.ApiClient.PostFormData<object>($"/videos/{videoId}/medias/{mediaType}", file);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDb = await fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb.Banner.Should().NotBeNull();
        videoFromDb.Banner!.Path.Should().Be(expectedFileName);
        fixture.WebAppFactory.StorageClient.Verify(
            x => x.Upload(
                expectedFileName,
                It.Is<Stream>(stream => stream.ToContentString() == expectedContent),
                file.ContentType,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = nameof(UploadThumb))]
    public async Task UploadThumb()
    {
        var exampleVideos = fixture.GetVideoCollection(5);
        await fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = exampleVideos[2].Id;
        var mediaType = "thumbnail";
        var file = fixture.GetValidImageFileInput();
        var expectedFileName = StorageFileName.Create(videoId,
            nameof(Video.Thumb), file.Extension);
        var expectedContent = file.FileStream.ToContentString();

        var (response, output) = await fixture.ApiClient
            .PostFormData<object>(
                $"/videos/{videoId}/medias/{mediaType}",
                file);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDb = await fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb.Thumb.Should().NotBeNull();
        videoFromDb.Thumb!.Path.Should().Be(expectedFileName);
        fixture.WebAppFactory.StorageClient.Verify(
            x => x.Upload(
                expectedFileName,
                It.Is<Stream>(stream => stream.ToContentString() == expectedContent),
                file.ContentType,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = nameof(UploadThumbHalf))]
    public async Task UploadThumbHalf()
    {
        var exampleVideos = fixture.GetVideoCollection(5);
        await fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = exampleVideos[2].Id;
        var mediaType = "thumbnail_half";
        var file = fixture.GetValidImageFileInput();
        var expectedFileName = StorageFileName.Create(videoId,
            nameof(Video.ThumbHalf), file.Extension);
        var expectedContent = file.FileStream.ToContentString();

        var (response, output) = await fixture.ApiClient
            .PostFormData<object>(
                $"/videos/{videoId}/medias/{mediaType}",
                file);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDb = await fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb.ThumbHalf.Should().NotBeNull();
        videoFromDb.ThumbHalf!.Path.Should().Be(expectedFileName);
        fixture.WebAppFactory.StorageClient.Verify(
            x => x.Upload(
                expectedFileName,
                It.Is<Stream>(stream => stream.ToContentString() == expectedContent),
                file.ContentType,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = nameof(UploadTrailer))]
    public async Task UploadTrailer()
    {
        var exampleVideos = fixture.GetVideoCollection(5);
        await fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = exampleVideos[2].Id;
        var mediaType = "trailer";
        var file = fixture.GetValidMediaFileInput();
        var expectedFileName = StorageFileName.Create(videoId,
            nameof(Video.Trailer), file.Extension);
        var expectedContent = file.FileStream.ToContentString();

        var (response, output) = await fixture.ApiClient
            .PostFormData<object>(
                $"/videos/{videoId}/medias/{mediaType}",
                file);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDb = await fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb.Trailer.Should().NotBeNull();
        videoFromDb.Trailer!.FilePath.Should().Be(expectedFileName);
        fixture.WebAppFactory.StorageClient.Verify(
            x => x.Upload(
                expectedFileName,
                It.Is<Stream>(stream => stream.ToContentString() == expectedContent),
                file.ContentType,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = nameof(UploadVideo))]
    public async Task UploadVideo()
    {
        var exampleVideos = fixture.GetVideoCollection(5);
        await fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = exampleVideos[2].Id;
        const string mediaType = "video";
        var file = fixture.GetValidMediaFileInput();
        var expectedFileName = StorageFileName.Create(videoId,
            nameof(Video.Media), file.Extension);
        var expectedContent = file.FileStream.ToContentString();

        (HttpResponseMessage? response, object? output) = await fixture.ApiClient.PostFormData<object>(
            $"/videos/{videoId}/medias/{mediaType}",
            file);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDb = await fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb.Media.Should().NotBeNull();
        videoFromDb.Media!.FilePath.Should().Be(expectedFileName);
        fixture.WebAppFactory.StorageClient.Verify(
            x => x.Upload(
                expectedFileName,
                It.Is<Stream>(stream => stream.ToContentString() == expectedContent),
                file.ContentType,
                It.IsAny<CancellationToken>()),
            Times.Once);
        (VideoUploadedEvent? @event, uint remainingMessages) = fixture.ReadMessageFromRabbitMQ<VideoUploadedEvent>();
        remainingMessages.Should().Be(0);
        @event.Should().NotBeNull();
        @event.FilePath.Should().Be(expectedFileName);
        @event.ResourceId.Should().Be(videoId);
        @event.OccurredOn.Should().NotBe(default);
        fixture.PurgeRabbitMQQueues();
    }

    [Fact(DisplayName = nameof(Error422WhenMediaTypeIsInvalid))]
    public async Task Error422WhenMediaTypeIsInvalid()
    {
        var exampleVideos = fixture.GetVideoCollection(5);
        await fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = exampleVideos[2].Id;
        var mediaType = "thumb";
        var file = fixture.GetValidImageFileInput();

        var (response, output) = await fixture.ApiClient
            .PostFormData<ProblemDetails>(
                $"/videos/{videoId}/medias/{mediaType}",
                file);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output.Type.Should().Be("UnprocessableEntity");
        output.Detail.Should().Be($"'{mediaType}' is not a valid media type.");
    }

    [Fact(DisplayName = nameof(Error404WhenVideoIdIsNotFound))]
    public async Task Error404WhenVideoIdIsNotFound()
    {
        var exampleVideos = fixture.GetVideoCollection(5);
        await fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = Guid.NewGuid();
        const string mediaType = "banner";
        var file = fixture.GetValidImageFileInput();

        (HttpResponseMessage? response, ProblemDetails? output) = await fixture.ApiClient
            .PostFormData<ProblemDetails>(
                $"/videos/{videoId}/medias/{mediaType}",
                file);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Video '{videoId}' not found.");
    }

    public void Dispose() => fixture.CleanPersistence();
}