using System.Net;

using FC.Codeflix.Catalog.EndToEndTests.Api.Videos.Common;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Videos.DeleteVideo;

public class DeleteVideoApiTest(VideoBaseFixture fixture) : IClassFixture<VideoBaseFixture>, IDisposable
{
    [Fact(DisplayName = nameof(DeleteVideo))]
    [Trait("EndToEnd/Api", "Video/DeleteVideo - Endpoints")]
    public async Task DeleteVideo()
    {
        var examples = fixture.GetVideoCollection(10);
        await fixture.VideoPersistence.InsertList(examples);
        var mediaCount = await fixture.VideoPersistence.GetMediaCount();
        var expectedMediaCount = mediaCount - 2;
        var video = examples[7];
        var videoId = video.Id;
        var allMedias = new[]
        {
            video.Trailer!.FilePath, video.Media!.FilePath, video.Banner!.Path, video.Thumb!.Path,
            video.ThumbHalf!.Path
        };

        var (response, output) = await fixture.ApiClient
            .Delete<object>($"/videos/{videoId}");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDB = await fixture.VideoPersistence
            .GetById(videoId);
        videoFromDB.Should().BeNull();
        var actualMediaCount = await fixture.VideoPersistence
            .GetMediaCount();
        actualMediaCount.Should().Be(expectedMediaCount);
        fixture.WebAppFactory.StorageClient.Verify(x => x.Delete(
                It.Is<string>(fileName => allMedias.Contains(fileName)),
                It.IsAny<CancellationToken>()),
            Times.Exactly(5));
        fixture.WebAppFactory.StorageClient.Verify(x => x.Delete(
                It.Is<string>(fileName => allMedias.Contains(fileName)),
                It.IsAny<CancellationToken>()),
            Times.Exactly(5));
    }

    [Fact(DisplayName = nameof(DeleteVideoWithRelationships))]
    [Trait("EndToEnd/Api", "Video/DeleteVideo - Endpoints")]
    public async Task DeleteVideoWithRelationships()
    {
        var exampleCategories = fixture.GetExampleCategoriesList(3);
        var exampleGenres = fixture.GetExampleListGenres(4);
        var exampleCastMembers = fixture.GetExampleCastMembersList(5);
        var exampleVideos = fixture.GetVideoCollection(10);

        exampleVideos.ForEach(video =>
        {
            exampleCategories.ForEach(category
                => video.AddCategory(category.Id));
            exampleGenres.ForEach(genre => video.AddGenre(genre.Id));
            exampleCastMembers.ForEach(castMember
                => video.AddCastMember(castMember.Id));
        });

        await fixture.CategoryPersistence.InsertList(exampleCategories);
        await fixture.GenrePersistence.InsertList(exampleGenres);
        await fixture.CastMemberPersistence.InsertList(exampleCastMembers);
        await fixture.VideoPersistence.InsertList(exampleVideos);
        var mediaCount = await fixture.VideoPersistence.GetMediaCount();
        var expectedMediaCount = mediaCount - 2;

        var video = exampleVideos[7];
        var videoId = video.Id;
        var allMedias = new[]
        {
            video.Trailer!.FilePath, video.Media!.FilePath, video.Banner!.Path, video.Thumb!.Path,
            video.ThumbHalf!.Path
        };

        var (response, output) = await fixture.ApiClient
            .Delete<object>($"/videos/{videoId}");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDB = await fixture.VideoPersistence
            .GetById(videoId);
        videoFromDB.Should().BeNull();
        var categoriesFromDB = await fixture.VideoPersistence
            .GetVideosCategories(videoId);
        categoriesFromDB.Should().BeEmpty();
        var genresFromDB = await fixture.VideoPersistence
            .GetVideosGenres(videoId);
        genresFromDB.Should().BeEmpty();
        var castMembersFromDB = await fixture.VideoPersistence
            .GetVideosCastMembers(videoId);
        castMembersFromDB.Should().BeEmpty();
        var actualMediaCount = await fixture.VideoPersistence
            .GetMediaCount();
        actualMediaCount.Should().Be(expectedMediaCount);
        fixture.WebAppFactory.StorageClient.Verify(
            x => x.Delete(
                It.Is<string>(fileName => allMedias.Contains(fileName)),
                It.IsAny<CancellationToken>()),
            Times.Exactly(5));
        fixture.WebAppFactory.StorageClient.Verify(
            x => x.Delete(
                It.Is<string>(fileName => allMedias.Contains(fileName)),
                It.IsAny<CancellationToken>()),
            Times.Exactly(5));
    }

    [Fact(DisplayName = nameof(Error404WhenVideoIdNotFound))]
    [Trait("EndToEnd/Api", "Video/DeleteVideo - Endpoints")]
    public async Task Error404WhenVideoIdNotFound()
    {
        var examples = fixture.GetVideoCollection(2);
        await fixture.VideoPersistence.InsertList(examples);

        var videoId = Guid.NewGuid();

        var (response, output) = await fixture.ApiClient
            .Delete<ProblemDetails>($"/videos/{videoId}");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Video '{videoId}' not found.");
    }

    public void Dispose() => fixture.CleanPersistence();
}