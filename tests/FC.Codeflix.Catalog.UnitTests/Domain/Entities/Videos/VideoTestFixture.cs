using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.UnitTests.Common;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.Videos;

public sealed class VideoTestFixture : BaseFixture
{
    public Video GetValidVideo() => new(
        GetValidTitle(),
        GetValidDescription(),
        GetValidYearLaunched(),
        GetValidDuration(),
        GetRandomBoolean(),
        GetRandomBoolean(),
        GetRandomRating()
    );

    public Rating GetRandomRating() => Faker.PickRandom<Rating>();
    public string GetTooLongTitle() => Faker.Lorem.Letter(400);
    public string GetValidTitle() => Faker.Lorem.Letter(100);
    public string GetValidDescription() => Faker.Commerce.ProductDescription();
    public string GetTooLongDescription() => Faker.Lorem.Letter(4001);
    public int GetValidYearLaunched() => Faker.Date.Between(new DateTime(1998, 10, 10), DateTime.Now).Year;
    public int GetValidDuration() => Faker.Random.Int(100, 300);
    public string GetValidImagePath() => Faker.Image.PicsumUrl();
}