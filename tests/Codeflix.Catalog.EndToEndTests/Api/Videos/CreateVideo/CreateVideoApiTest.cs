using System.Net;

using Codeflix.Catalog.Api.Models.Responses;
using Codeflix.Catalog.Api.Models.Videos;
using Codeflix.Catalog.EndToEndTests.Api.Videos.Common;

using Codeflix.Catalog.Application.UseCases.Videos.Common;
using Codeflix.Catalog.Domain.Extensions;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codeflix.Catalog.EndToEndTests.Api.Videos.CreateVideo;

[Trait("EndToEnd/Api", "Video/CreateVideo - Endpoints")]
public sealed class CreateVideoApiTest(VideoBaseFixture fixture) : IClassFixture<VideoBaseFixture>, IDisposable
{
    [Fact(DisplayName = nameof(CreateBasicVideo))]
    public async Task CreateBasicVideo()
    {
        CreateVideoApiInput input = fixture.GetBasicCreateVideoInput();

        var (response, output) = await fixture.ApiClient
            .Post<ApiResponse<VideoModelOutput>>("/videos", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Title.Should().Be(input.Title);
        output.Data.Description.Should().Be(input.Description);
        output.Data.Title.Should().Be(input.Title);
        output.Data.YearLaunched.Should().Be(input.YearLaunched);
        output.Data.Opened.Should().Be(input.Opened);
        output.Data.Published.Should().Be(input.Published);
        output.Data.Duration.Should().Be(input.Duration);
        output.Data.Rating.Should().Be(input.Rating);
        var videoFromDb = await fixture.VideoPersistence.GetById(output.Data.Id);
        videoFromDb.Should().NotBeNull();
        videoFromDb.Id.Should().NotBeEmpty();
        videoFromDb.Title.Should().Be(input.Title);
        videoFromDb.Description.Should().Be(input.Description);
        videoFromDb.YearLaunched.Should().Be(input.YearLaunched);
        videoFromDb.Opened.Should().Be(input.Opened);
        videoFromDb.Published.Should().Be(input.Published);
        videoFromDb.Duration.Should().Be(input.Duration);
        videoFromDb.Rating.Should().Be(input.Rating!.ToRating());
    }

    [Fact(DisplayName = nameof(CreateVideoWithRelationships))]
    [Trait("EndToEnd/Api", "Video/CreateVideo - Endpoints")]
    public async Task CreateVideoWithRelationships()
    {
        var categories = fixture.GetExampleCategoriesList();
        await fixture.CategoryPersistence.InsertList(categories);

        var genres = fixture.GetExampleListGenres();
        await fixture.GenrePersistence.InsertList(genres);

        var castMembers = fixture.GetExampleCastMembersList();
        await fixture.CastMemberPersistence.InsertList(castMembers);

        CreateVideoApiInput input = fixture.GetBasicCreateVideoInput();
        input.CategoriesIds = categories.Select(c => c.Id).ToList();
        input.GenresIds = genres.Select(c => c.Id).ToList();
        input.CastMembersIds = castMembers.Select(c => c.Id).ToList();

        var (response, output) = await fixture.ApiClient
            .Post<ApiResponse<VideoModelOutput>>("/videos", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Title.Should().Be(input.Title);
        output.Data.Description.Should().Be(input.Description);
        output.Data.Title.Should().Be(input.Title);
        output.Data.YearLaunched.Should().Be(input.YearLaunched);
        output.Data.Opened.Should().Be(input.Opened);
        output.Data.Published.Should().Be(input.Published);
        output.Data.Duration.Should().Be(input.Duration);
        output.Data.Rating.Should().Be(input.Rating);
        var outputCategoryIds = output.Data.Categories.Select(c => c.Id).ToList();
        outputCategoryIds.Should().NotBeEmpty();
        outputCategoryIds.Should().BeEquivalentTo(input.CategoriesIds);
        var outputGenreIds = output.Data.Genres.Select(c => c.Id).ToList();
        outputGenreIds.Should().NotBeEmpty();
        outputGenreIds.Should().BeEquivalentTo(input.GenresIds);
        var outputCastMemberIds = output.Data.CastMembers.Select(c => c.Id).ToList();
        outputCastMemberIds.Should().NotBeEmpty();
        outputCastMemberIds.Should().BeEquivalentTo(input.CastMembersIds);
        var videoFromDb = await fixture.VideoPersistence.GetById(output.Data.Id);
        videoFromDb.Should().NotBeNull();
        videoFromDb.Id.Should().NotBeEmpty();
        videoFromDb.Title.Should().Be(input.Title);
        videoFromDb.Description.Should().Be(input.Description);
        videoFromDb.YearLaunched.Should().Be(input.YearLaunched);
        videoFromDb.Opened.Should().Be(input.Opened);
        videoFromDb.Published.Should().Be(input.Published);
        videoFromDb.Duration.Should().Be(input.Duration);
        videoFromDb.Rating.Should().Be(input.Rating!.ToRating());
        var categoriesFromDb = await fixture.VideoPersistence
            .GetVideosCategories(videoFromDb.Id);
        categoriesFromDb.Should().NotBeNull();
        var categoriesIdsFromDb = categoriesFromDb.Select(x => x.CategoryId);
        categoriesIdsFromDb.Should().BeEquivalentTo(input.CategoriesIds);
        var genresFromDb = await fixture.VideoPersistence
            .GetVideosGenres(videoFromDb.Id);
        genresFromDb.Should().NotBeNull();
        var genresIdsFromDb = genresFromDb.Select(x => x.GenreId);
        genresIdsFromDb.Should().BeEquivalentTo(input.GenresIds);
        var castMembersFromDb = await fixture.VideoPersistence
            .GetVideosCastMembers(videoFromDb.Id);
        castMembersFromDb.Should().NotBeNull();
        var castMembersIdsFromDb = castMembersFromDb.Select(x => x.CastMemberId);
        castMembersIdsFromDb.Should().BeEquivalentTo(input.CastMembersIds);
    }

    [Fact(DisplayName = nameof(CreateVideoWithInvalidGenreId))]
    [Trait("EndToEnd/Api", "Video/CreateVideo - Endpoints")]
    public async Task CreateVideoWithInvalidGenreId()
    {
        var invalidGenreId = Guid.NewGuid();
        CreateVideoApiInput input = fixture.GetBasicCreateVideoInput();
        input.GenresIds = new List<Guid> { invalidGenreId };

        var (response, output) = await fixture.ApiClient
            .Post<ProblemDetails>("/videos", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related genre id (or ids) not found: {invalidGenreId}.");
    }

    [Fact(DisplayName = nameof(CreateVideoWithInvalidCategoryId))]
    [Trait("EndToEnd/Api", "Video/CreateVideo - Endpoints")]
    public async Task CreateVideoWithInvalidCategoryId()
    {
        var invalidCategoryId = Guid.NewGuid();
        CreateVideoApiInput input = fixture.GetBasicCreateVideoInput();
        input.CategoriesIds = new List<Guid> { invalidCategoryId };

        var (response, output) = await fixture.ApiClient
            .Post<ProblemDetails>("/videos", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related category id (or ids) not found: {invalidCategoryId}.");
    }

    [Fact(DisplayName = nameof(CreateVideoWithInvalidCastMemberId))]
    [Trait("EndToEnd/Api", "Video/CreateVideo - Endpoints")]
    public async Task CreateVideoWithInvalidCastMemberId()
    {
        var invalidCastMemberId = Guid.NewGuid();
        CreateVideoApiInput input = fixture.GetBasicCreateVideoInput();
        input.CastMembersIds = [invalidCastMemberId];

        var (response, output) = await fixture.ApiClient
            .Post<ProblemDetails>("/videos", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related cast member id (or ids) not found: {invalidCastMemberId}.");
    }


    public void Dispose() => fixture.CleanPersistence();
}