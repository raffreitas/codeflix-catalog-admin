using System.Net;

using Codeflix.Catalog.EndToEndTests.Api.Videos.Common;
using Codeflix.Catalog.EndToEndTests.Models;

using Codeflix.Catalog.Application.UseCases.Videos.Common;
using Codeflix.Catalog.Domain.Extensions;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

namespace Codeflix.Catalog.EndToEndTests.Api.Videos.GetVideo;

public class GetVideoApiTest(VideoBaseFixture fixture) : IClassFixture<VideoBaseFixture>, IDisposable
{
    [Fact(DisplayName = nameof(GetVideo))]
    [Trait("EndToEnd/Api", "Video/GetVideo - Endpoints")]
    public async Task GetVideo()
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

        var exampleItem = exampleVideos.ElementAt(5);

        var (response, output) = await fixture.ApiClient
            .Get<TestApiResponse<VideoModelOutput>>($"/videos/{exampleItem.Id}");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data!.Id.Should().Be(exampleItem.Id);
        output.Data.Title.Should().Be(exampleItem.Title);
        output.Data.Description.Should().Be(exampleItem.Description);
        output.Data.YearLaunched.Should().Be(exampleItem.YearLaunched);
        output.Data.Opened.Should().Be(exampleItem.Opened);
        output.Data.Published.Should().Be(exampleItem.Published);
        output.Data.Duration.Should().Be(exampleItem.Duration);
        output.Data.Rating.Should().Be(exampleItem.Rating.ToStringSignal());
        var expectedCategories = exampleCategories
            .Select(category => new VideoModelOutputRelatedAggregate(
                category.Id, null));
        output.Data.Categories.Should().BeEquivalentTo(expectedCategories);
        var expectedGenres = exampleGenres
            .Select(genre => new VideoModelOutputRelatedAggregate(
                genre.Id, null));
        output.Data.Genres.Should().BeEquivalentTo(expectedGenres);
        var expectedCastMembers = exampleCastMembers
            .Select(castMember => new VideoModelOutputRelatedAggregate(
                castMember.Id, null));
        output.Data.CastMembers.Should().BeEquivalentTo(expectedCastMembers);
    }

    [Fact(DisplayName = nameof(Error404WhenIdNotFound))]
    [Trait("EndToEnd/Api", "Video/GetVideo - Endpoints")]
    public async Task Error404WhenIdNotFound()
    {
        var exampleVideos = fixture.GetVideoCollection(10);
        await fixture.VideoPersistence.InsertList(exampleVideos);
        var exampleVideoId = Guid.NewGuid();

        var (response, output) = await fixture.ApiClient
            .Get<ProblemDetails>($"/videos/{exampleVideoId}");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Video '{exampleVideoId}' not found.");
    }

    public void Dispose() => fixture.CleanPersistence();
}