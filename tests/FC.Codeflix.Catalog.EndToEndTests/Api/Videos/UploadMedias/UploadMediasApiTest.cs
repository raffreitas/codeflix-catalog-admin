using System.Net;

using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.EndToEndTests.Api.Videos.Common;
using FC.Codeflix.Catalog.EndToEndTests.Extensions;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Videos.UploadMedias;

public class UploadMediasApiTest : IClassFixture<VideoBaseFixture>, IDisposable
{
    private readonly VideoBaseFixture _fixture;

    public UploadMediasApiTest(VideoBaseFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(UploadBanner))]
    [Trait("EndToEnd/Api", "Video/UploadMedias - Endpoints")]
    public async Task UploadBanner()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = exampleVideos[2].Id;
        var mediaType = "banner";
        var file = _fixture.GetValidImageFileInput();
        var expectedFileName = StorageFileName.Create(videoId, nameof(Video.Banner), file.Extension);
        var expectedContent = file.FileStream.ToContentString();

        var (response, output) =
            await _fixture.ApiClient.PostFormData<object>($"/videos/{videoId}/medias/{mediaType}", file);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDb = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb.Banner!.Path.Should().Be(expectedFileName);
        _fixture.WebAppFactory.StorageClient.Verify(
            x => x.Upload(
                expectedFileName,
                It.Is<Stream>(stream => stream.ToContentString() == expectedContent),
                file.ContentType,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = nameof(UploadThumb))]
    [Trait("EndToEnd/Api", "Video/UploadMedias - Endpoints")]
    public async Task UploadThumb()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = exampleVideos[2].Id;
        var mediaType = "thumbnail";
        var file = _fixture.GetValidImageFileInput();
        var expectedFileName = StorageFileName.Create(videoId,
            nameof(Video.Thumb), file.Extension);
        var expectedContent = file.FileStream.ToContentString();

        var (response, output) = await _fixture.ApiClient
            .PostFormData<object>(
                $"/videos/{videoId}/medias/{mediaType}",
                file);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDb = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb.Thumb!.Path.Should().Be(expectedFileName);
        _fixture.WebAppFactory.StorageClient.Verify(
            x => x.Upload(
                expectedFileName,
                It.Is<Stream>(stream => stream.ToContentString() == expectedContent),
                file.ContentType,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = nameof(UploadThumbHalf))]
    [Trait("EndToEnd/Api", "Video/UploadMedias - Endpoints")]
    public async Task UploadThumbHalf()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = exampleVideos[2].Id;
        var mediaType = "thumbnail_half";
        var file = _fixture.GetValidImageFileInput();
        var expectedFileName = StorageFileName.Create(videoId,
            nameof(Video.ThumbHalf), file.Extension);
        var expectedContent = file.FileStream.ToContentString();

        var (response, output) = await _fixture.ApiClient
            .PostFormData<object>(
                $"/videos/{videoId}/medias/{mediaType}",
                file);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDb = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb.ThumbHalf!.Path.Should().Be(expectedFileName);
        _fixture.WebAppFactory.StorageClient.Verify(
            x => x.Upload(
                expectedFileName,
                It.Is<Stream>(stream => stream.ToContentString() == expectedContent),
                file.ContentType,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = nameof(UploadTrailer))]
    [Trait("EndToEnd/Api", "Video/UploadMedias - Endpoints")]
    public async Task UploadTrailer()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = exampleVideos[2].Id;
        var mediaType = "trailer";
        var file = _fixture.GetValidMediaFileInput();
        var expectedFileName = StorageFileName.Create(videoId,
            nameof(Video.Trailer), file.Extension);
        var expectedContent = file.FileStream.ToContentString();

        var (response, output) = await _fixture.ApiClient
            .PostFormData<object>(
                $"/videos/{videoId}/medias/{mediaType}",
                file);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDb = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb.Trailer!.FilePath.Should().Be(expectedFileName);
        _fixture.WebAppFactory.StorageClient.Verify(
            x => x.Upload(
                expectedFileName,
                It.Is<Stream>(stream => stream.ToContentString() == expectedContent),
                file.ContentType,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = nameof(UploadVideo))]
    [Trait("EndToEnd/Api", "Video/UploadMedias - Endpoints")]
    public async Task UploadVideo()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);
        _fixture.SetupRabbitMQ();

        var videoId = exampleVideos[2].Id;
        var mediaType = "video";
        var file = _fixture.GetValidMediaFileInput();
        var expectedFileName = StorageFileName.Create(videoId,
            nameof(Video.Media), file.Extension);
        var expectedContent = file.FileStream.ToContentString();

        var (response, output) = await _fixture.ApiClient.PostFormData<object>(
            $"/videos/{videoId}/medias/{mediaType}",
            file);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDb = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb.Media!.FilePath.Should().Be(expectedFileName);
        _fixture.WebAppFactory.StorageClient.Verify(
            x => x.Upload(
                expectedFileName,
                It.Is<Stream>(stream => stream.ToContentString() == expectedContent),
                file.ContentType,
                It.IsAny<CancellationToken>()),
            Times.Once);
        var (@event, remainingMessages) = _fixture.ReadMessageFromRabbitMQ();
        remainingMessages.Should().Be(0);
        @event.Should().NotBeNull();
        @event.FilePath.Should().Be(expectedFileName);
        @event.ResourceId.Should().Be(videoId);
        @event.OccurredOn.Should().NotBe(default);
        _fixture.TearDownRabbitMQ();
    }

    [Fact(DisplayName = nameof(Error422WhenMediaTypeIsInvalid))]
    [Trait("EndToEnd/Api", "Video/UploadMedias - Endpoints")]
    public async Task Error422WhenMediaTypeIsInvalid()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = exampleVideos[2].Id;
        var mediaType = "thumb";
        var file = _fixture.GetValidImageFileInput();

        var (response, output) = await _fixture.ApiClient
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
    [Trait("EndToEnd/Api", "Video/UploadMedias - Endpoints")]
    public async Task Error404WhenVideoIdIsNotFound()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = Guid.NewGuid();
        var mediaType = "banner";
        var file = _fixture.GetValidImageFileInput();

        var (response, output) = await _fixture.ApiClient
            .PostFormData<ProblemDetails>(
                $"/videos/{videoId}/medias/{mediaType}",
                file);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Video '{videoId}' not found.");
    }

    public void Dispose() => _fixture.CleanPersistence();
}