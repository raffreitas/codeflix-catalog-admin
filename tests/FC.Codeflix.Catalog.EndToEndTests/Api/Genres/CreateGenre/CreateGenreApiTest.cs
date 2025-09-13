using System.Net;

using FC.Codeflix.Catalog.Api.Models.Responses;
using FC.Codeflix.Catalog.Application.UseCases.Genres.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genres.CreateGenre;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genres.CreateGenre;

[Collection(nameof(CreateGenreApiTestFixture))]
public class CreateGenreApiTest : IDisposable
{
    private readonly CreateGenreApiTestFixture _fixture;

    public CreateGenreApiTest(CreateGenreApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("EndToEnd/Api", "Genre/CreateGenre - Endpoints")]
    public async Task CreateGenre()
    {
        var apiInput = new CreateGenreInput(
            _fixture.GetValidCategoryName(),
            _fixture.GetRandomBoolean()
        );

        var (response, output) = await _fixture.ApiClient
            .Post<ApiResponse<GenreModelOutput>>("/genres", apiInput);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(apiInput.Name);
        output.Data.IsActive.Should().Be(apiInput.IsActive);
        output.Data.Categories.Should().HaveCount(0);
        var genreFromDb = await _fixture.GenrePersistence.GetById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb.Name.Should().Be(apiInput.Name);
        genreFromDb.IsActive.Should().Be(apiInput.IsActive);
    }

    [Fact(DisplayName = nameof(CreateGenreWithRelations))]
    [Trait("EndToEnd/Api", "Genre/CreateGenre - Endpoints")]
    public async Task CreateGenreWithRelations()
    {
        var exampleCategories = _fixture.GetExampleCategoriesList();
        await _fixture.CategoryPersistence.InsertList(exampleCategories);
        var relatedCategories = exampleCategories
            .Skip(3).Take(3).Select(x => x.Id).ToList();

        var apiInput = new CreateGenreInput(
            _fixture.GetValidCategoryName(),
            _fixture.GetRandomBoolean(),
            relatedCategories
        );

        var (response, output) = await _fixture.ApiClient
            .Post<ApiResponse<GenreModelOutput>>("/genres", apiInput);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(apiInput.Name);
        output.Data.IsActive.Should().Be(apiInput.IsActive);
        output.Data.Categories.Should().HaveCount(relatedCategories.Count);
        var outputRelatedCategoryIds = output.Data.Categories.Select(x => x.Id).ToList();
        outputRelatedCategoryIds.Should().BeEquivalentTo(relatedCategories);
        var genreFromDb = await _fixture.GenrePersistence.GetById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb.Name.Should().Be(apiInput.Name);
        genreFromDb.IsActive.Should().Be(apiInput.IsActive);
        var relationsFromDb = await _fixture.GenrePersistence
            .GetGenresCategoriesRelationsByGenreId(output.Data.Id);
        relationsFromDb.Should().NotBeNull();
        relationsFromDb.Should().HaveCount(relatedCategories.Count);
        var relatedCategoriesIdsFromDb =
            relationsFromDb.Select(x => x.CategoryId).ToList();
        relatedCategoriesIdsFromDb.Should().BeEquivalentTo(relatedCategories);
    }

    [Fact(DisplayName = nameof(ErrorWithInvalidRelations))]
    [Trait("EndToEnd/Api", "Genre/CreateGenre - Endpoints")]
    public async Task ErrorWithInvalidRelations()
    {
        var exampleCategories = _fixture.GetExampleCategoriesList();
        await _fixture.CategoryPersistence.InsertList(exampleCategories);
        var relatedCategories = exampleCategories
            .Skip(3).Take(3).Select(x => x.Id).ToList();
        var invalidCategoryId = Guid.NewGuid();
        relatedCategories.Add(invalidCategoryId);
        var apiInput = new CreateGenreInput(
            _fixture.GetValidCategoryName(),
            _fixture.GetRandomBoolean(),
            relatedCategories
        );

        var (response, output) = await _fixture.ApiClient
            .Post<ProblemDetails>("/genres", apiInput);

        response.Should().NotBeNull();
        response.StatusCode.Should()
            .Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
        output.Should().NotBeNull();
        output.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related category id (or ids) not found: {invalidCategoryId}");
    }

    public void Dispose() => _fixture.CleanPersistence();
}