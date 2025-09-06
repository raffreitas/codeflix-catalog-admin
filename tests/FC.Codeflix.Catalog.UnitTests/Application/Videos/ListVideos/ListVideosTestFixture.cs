using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;

namespace FC.Codeflix.Catalog.UnitTests.Application.Videos.ListVideos;

public sealed class ListVideosTestFixture : VideoTestFixtureBase
{
    public List<Video> CreateExampleVideosList()
        => Enumerable.Range(1, Random.Shared.Next(2, 10))
            .Select(_ => GetValidVideoWithAllProperties())
            .ToList();

    public List<Video> CreateExampleVideosListWithoutRelations()
        => Enumerable.Range(1, Random.Shared.Next(2, 10))
            .Select(_ => GetValidVideo())
            .ToList();

    public (
        List<Video> Videos,
        List<Category> Categories,
        List<Genre> Genres
        ) CreateExampleVideosListWithRelations()
    {
        var itemsQuantityToBeCreated = Random.Shared.Next(2, 10);
        List<Category> categories = [];
        List<Genre> genres = [];
        var videos = Enumerable.Range(1, itemsQuantityToBeCreated)
            .Select(_ => GetValidVideoWithAllProperties())
            .ToList();

        videos.ForEach(video =>
        {
            video.RemoveAllCategories();
            var qtdCategories = Random.Shared.Next(2, 5);
            for (var i = 0; i < qtdCategories; i++)
            {
                var category = GetExampleCategory();
                categories.Add(category);
                video.AddCategory(category.Id);
            }

            video.RemoveAllGenres();
            var qtdGenres = Random.Shared.Next(2, 5);
            for (var i = 0; i < qtdGenres; i++)
            {
                var genre = GetExampleGenre();
                genres.Add(genre);
                video.AddGenre(genre.Id);
            }
        });

        return (videos, categories, genres);
    }

    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if (categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription =
            Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10_000)
            categoryDescription =
                categoryDescription[..10_000];
        return categoryDescription;
    }

    public Category GetExampleCategory() => new(
        GetValidCategoryName(),
        GetValidCategoryDescription(),
        GetRandomBoolean()
    );

    string GetValidGenreName()
        => Faker.Commerce.Categories(1)[0];

    public Genre GetExampleGenre() => new(
        GetValidGenreName(),
        GetRandomBoolean()
    );
}