using System.Net;

using Codeflix.Catalog.Api.Models.Genres;
using Codeflix.Catalog.Api.Models.Responses;
using Codeflix.Catalog.Application.UseCases.Genres.Common;
using Codeflix.Catalog.Infra.Data.EF.Models;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using DomainEntity = Codeflix.Catalog.Domain.Entities;

namespace Codeflix.Catalog.EndToEndTests.Api.Genres.UpdateGenre;

[Collection(nameof(UpdateGenreApiTestFixture))]
public class UpdateGenreApiTest(UpdateGenreApiTestFixture fixture) : IDisposable
{
    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("EndToEnd/Api", "Genre/UpdateGenre - Endpoints")]
    public async Task UpdateGenre()
    {
        List<DomainEntity.Genre> exampleGenres = fixture.GetExampleListGenres();
        var targetGenre = exampleGenres[5];
        await fixture.GenrePersistence.InsertList(exampleGenres);
        var input = new UpdateGenreApiInput(
            fixture.GetValidGenreName(),
            fixture.GetRandomBoolean()
        );

        var (response, output) = await fixture.ApiClient
            .Put<ApiResponse<GenreModelOutput>>(
                $"/genres/{targetGenre.Id}",
                input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be((bool)input.IsActive!);
        var genreFromDb = await fixture.GenrePersistence.GetById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
    }

    [Fact(DisplayName = nameof(ProblemDetailsWhenNotFound))]
    [Trait("EndToEnd/Api", "Genre/UpdateGenre - Endpoints")]
    public async Task ProblemDetailsWhenNotFound()
    {
        List<DomainEntity.Genre> exampleGenres = fixture.GetExampleListGenres();
        var randomGuid = Guid.NewGuid();
        await fixture.GenrePersistence.InsertList(exampleGenres);
        var input = new UpdateGenreApiInput(
            fixture.GetValidGenreName(),
            fixture.GetRandomBoolean()
        );

        var (response, output) = await fixture.ApiClient
            .Put<ProblemDetails>(
                $"/genres/{randomGuid}",
                input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"Genre '{randomGuid}' not found.");
        output.Type.Should().Be("NotFound");
        output.Status.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact(DisplayName = nameof(UpdateGenreWithRelations))]
    [Trait("EndToEnd/Api", "Genre/UpdateGenre - Endpoints")]
    public async Task UpdateGenreWithRelations()
    {
        List<DomainEntity.Genre> exampleGenres = fixture.GetExampleListGenres();
        var targetGenre = exampleGenres[5];
        List<DomainEntity.Category> exampleCategories = fixture.GetExampleCategoriesList();
        Random random = new Random();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = random.Next(2, exampleCategories.Count - 1);
            for (int i = 0; i < relationsCount; i++)
            {
                int selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
                DomainEntity.Category selected = exampleCategories[selectedCategoryIndex];
                if (!genre.Categories.Contains(selected.Id))
                    genre.AddCategory(selected.Id);
            }
        });
        List<GenresCategories> genresCategories = new List<GenresCategories>();
        exampleGenres.ForEach(genre => genre.Categories.ToList().ForEach(categoryId => genresCategories.Add(
                    new GenresCategories(genre.Id, categoryId)
                )
            )
        );
        int newRelationsCount = random.Next(2, exampleCategories.Count - 1);
        var newRelatedCategoriesIds = new List<Guid>();
        for (int i = 0; i < newRelationsCount; i++)
        {
            int selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
            DomainEntity.Category selected = exampleCategories[selectedCategoryIndex];
            if (!newRelatedCategoriesIds.Contains(selected.Id))
                newRelatedCategoriesIds.Add(selected.Id);
        }

        await fixture.GenrePersistence.InsertList(exampleGenres);
        await fixture.CategoryPersistence.InsertList(exampleCategories);
        await fixture.GenrePersistence.InsertGenresCategoriesRelationsList(genresCategories);
        var input = new UpdateGenreApiInput(
            fixture.GetValidGenreName(),
            fixture.GetRandomBoolean(),
            newRelatedCategoriesIds
        );

        var (response, output) = await fixture.ApiClient
            .Put<ApiResponse<GenreModelOutput>>(
                $"/genres/{targetGenre.Id}",
                input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be((bool)input.IsActive!);
        List<Guid> relatedCategoriesIdsFromOutput =
            output.Data.Categories.Select(relation => relation.Id).ToList();
        relatedCategoriesIdsFromOutput.Should()
            .BeEquivalentTo(newRelatedCategoriesIds);
        var genreFromDb = await fixture.GenrePersistence.GetById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
        var genresCategoriesFromDb =
            await fixture.GenrePersistence.GetGenresCategoriesRelationsByGenreId(targetGenre.Id);
        var relatedCategoriesIdsFromDb =
            genresCategoriesFromDb
                .Select(x => x.CategoryId)
                .ToList();
        relatedCategoriesIdsFromDb.Should()
            .BeEquivalentTo(newRelatedCategoriesIds);
    }

    [Fact(DisplayName = nameof(PersistsRelationsWhenNotPresentInInput))]
    [Trait("EndToEnd/Api", "Genre/UpdateGenre - Endpoints")]
    public async Task PersistsRelationsWhenNotPresentInInput()
    {
        List<DomainEntity.Genre> exampleGenres = fixture.GetExampleListGenres();
        var targetGenre = exampleGenres[5];
        List<DomainEntity.Category> exampleCategories = fixture.GetExampleCategoriesList();
        Random random = new Random();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = random.Next(2, exampleCategories.Count - 1);
            for (int i = 0; i < relationsCount; i++)
            {
                int selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
                DomainEntity.Category selected = exampleCategories[selectedCategoryIndex];
                if (!genre.Categories.Contains(selected.Id))
                    genre.AddCategory(selected.Id);
            }
        });
        List<GenresCategories> genresCategories = new List<GenresCategories>();
        exampleGenres.ForEach(genre => genre.Categories.ToList().ForEach(categoryId => genresCategories.Add(
                    new GenresCategories(genre.Id, categoryId)
                )
            )
        );
        await fixture.GenrePersistence.InsertList(exampleGenres);
        await fixture.CategoryPersistence.InsertList(exampleCategories);
        await fixture.GenrePersistence.InsertGenresCategoriesRelationsList(genresCategories);
        var input = new UpdateGenreApiInput(
            fixture.GetValidGenreName(),
            fixture.GetRandomBoolean()
        );

        var (response, output) = await fixture.ApiClient
            .Put<ApiResponse<GenreModelOutput>>(
                $"/genres/{targetGenre.Id}",
                input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be((bool)input.IsActive!);
        List<Guid> relatedCategoriesIdsFromOutput =
            output.Data.Categories.Select(relation => relation.Id).ToList();
        relatedCategoriesIdsFromOutput.Should()
            .BeEquivalentTo(targetGenre.Categories);
        var genreFromDb = await fixture.GenrePersistence.GetById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
        var genresCategoriesFromDb =
            await fixture.GenrePersistence.GetGenresCategoriesRelationsByGenreId(targetGenre.Id);
        var relatedCategoriesIdsFromDb =
            genresCategoriesFromDb
                .Select(x => x.CategoryId)
                .ToList();
        relatedCategoriesIdsFromDb.Should()
            .BeEquivalentTo(targetGenre.Categories);
    }

    [Fact(DisplayName = nameof(ErrorWhenInvalidRelation))]
    [Trait("EndToEnd/Api", "Genre/UpdateGenre - Endpoints")]
    public async Task ErrorWhenInvalidRelation()
    {
        List<DomainEntity.Genre> exampleGenres = fixture.GetExampleListGenres();
        var targetGenre = exampleGenres[5];
        var randomGuid = Guid.NewGuid();
        await fixture.GenrePersistence.InsertList(exampleGenres);
        var input = new UpdateGenreApiInput(
            fixture.GetValidGenreName(),
            fixture.GetRandomBoolean(),
            new List<Guid> { randomGuid }
        );

        var (response, output) = await fixture.ApiClient
            .Put<ProblemDetails>(
                $"/genres/{targetGenre.Id}",
                input
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
        output.Should().NotBeNull();
        output.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related category id (or ids) not found: {randomGuid}");
    }

    public void Dispose() => fixture.CleanPersistence();
}