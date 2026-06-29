using System.Net;

using Codeflix.Catalog.Infra.Data.EF.Models;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using DomainEntity = Codeflix.Catalog.Domain.Entities;

namespace Codeflix.Catalog.EndToEndTests.Api.Genres.DeleteGenre;

[Collection(nameof(DeleteGenreTestApiFixture))]
public class DeleteGenreTestApi(DeleteGenreTestApiFixture fixture) : IDisposable
{
    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("EndToEnd/Api", "Genre/DeleteGenre - Endpoints")]
    public async Task DeleteGenre()
    {
        List<DomainEntity.Genre> exampleGenres = fixture.GetExampleListGenres();
        var targetGenre = exampleGenres[5];
        await fixture.GenrePersistence.InsertList(exampleGenres);

        var (response, output) = await fixture.ApiClient
            .Delete<object>($"/genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();
        DomainEntity.Genre? genreDb = await fixture.GenrePersistence.GetById(targetGenre.Id);
        genreDb.Should().BeNull();
    }

    [Fact(DisplayName = nameof(WhenNotFound404))]
    [Trait("EndToEnd/Api", "Genre/DeleteGenre - Endpoints")]
    public async Task WhenNotFound404()
    {
        List<DomainEntity.Genre> exampleGenres = fixture.GetExampleListGenres();
        var randomGuid = Guid.NewGuid();
        await fixture.GenrePersistence.InsertList(exampleGenres);

        var (response, output) = await fixture.ApiClient
            .Delete<ProblemDetails>($"/genres/{randomGuid}");

        response.Should().NotBeNull();
        output.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Genre '{randomGuid}' not found.");
    }


    [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
    [Trait("EndToEnd/Api", "Genre/DeleteGenre - Endpoints")]
    public async Task DeleteGenreWithRelations()
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

        var (response, output) = await fixture.ApiClient
            .Delete<object>($"/genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();
        DomainEntity.Genre? genreDb = await fixture.GenrePersistence.GetById(targetGenre.Id);
        genreDb.Should().BeNull();
        List<GenresCategories> relations =
            await fixture.GenrePersistence.GetGenresCategoriesRelationsByGenreId(targetGenre.Id);
        relations.Should().HaveCount(0);
    }

    public void Dispose() => fixture.CleanPersistence();
}