using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.UnitTests.Common;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.Videos;

public sealed class VideoTestFixture : BaseFixture
{
    public Video GetValidVideo() => new Video(
        GetValidTitle(),
        GetValidDescription(),
        GetValidYearLaunched(),
        GetValidDuration(),
        GetRandomBoolean(),
        GetRandomBoolean()
    );

    public string GetTooLongTitle() => Faker.Lorem.Letter(400);
    public string GetValidTitle() => Faker.Lorem.Letter(100);
    public string GetValidDescription() => Faker.Commerce.ProductDescription();
    public string GetTooLongDescription() => Faker.Lorem.Letter(4001);
    public int GetValidYearLaunched() => Faker.Date.Between(new DateTime(1998, 10, 10), DateTime.Now).Year;
    public int GetValidDuration() => Faker.Random.Int(100, 300);
}