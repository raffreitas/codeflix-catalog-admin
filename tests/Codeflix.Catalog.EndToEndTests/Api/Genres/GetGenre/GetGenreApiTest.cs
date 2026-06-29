using System.Net;

using Codeflix.Catalog.Api.Models.Responses;
using Codeflix.Catalog.Application.UseCases.Genres.Common;
using Codeflix.Catalog.Infra.Data.EF.Models;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using DomainEntity = Codeflix.Catalog.Domain.Entities;

namespace Codeflix.Catalog.EndToEndTests.Api.Genres.GetGenre;

[Collection(nameof(GetGenreApiTestFixture))]
public class GetGenreApiTest(GetGenreApiTestFixture fixture) : IDisposable
{
    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("EndToEnd/API", "Genre/GetGenre - Endpoints")]
    public async Task GetGenre()
    {
        List<DomainEntity.Genre> exampleGenres = fixture.GetExampleListGenres();
        var targetGenre = exampleGenres[5];
        await fixture.GenrePersistence.InsertList(exampleGenres);

        var (response, output) = await fixture.ApiClient
            .Get<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(targetGenre.Name);
        output.Data.IsActive.Should().Be(targetGenre.IsActive);
    }

    [Fact(DisplayName = nameof(NotFound))]
    [Trait("EndToEnd/API", "Genre/GetGenre - Endpoints")]
    public async Task NotFound()
    {
        List<DomainEntity.Genre> exampleGenres = fixture.GetExampleListGenres();
        var randomGuid = Guid.NewGuid();
        await fixture.GenrePersistence.InsertList(exampleGenres);

        var (response, output) = await fixture.ApiClient
            .Get<ProblemDetails>($"/genres/{randomGuid}");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Genre '{randomGuid}' not found.");
    }

    [Fact(DisplayName = nameof(GetGenreWithRelations))]
    [Trait("EndToEnd/API", "Genre/GetGenre - Endpoints")]
    public async Task GetGenreWithRelations()
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
            .Get<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(targetGenre.Name);
        output.Data.IsActive.Should().Be(targetGenre.IsActive);
        foreach (var category in output.Data.Categories)
        {
            var expectedCategory = exampleCategories.Find(x => x.Id == category.Id);
            category.Name.Should().Be(expectedCategory!.Name);
        }
    }

    public void Dispose() => fixture.CleanPersistence();
}