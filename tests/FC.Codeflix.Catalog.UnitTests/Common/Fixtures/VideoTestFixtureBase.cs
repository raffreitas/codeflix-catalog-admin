using System.Text;

using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.UnitTests.Common.Fixtures;

public abstract class VideoTestFixtureBase : BaseFixture
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
    public string GetValidMediaPath() => Faker.Internet.UrlWithPath(fileExt: "mp4");
    public Media GetValidMedia() => new(GetValidMediaPath());

    public FileInput GetValidImageFileInput()
    {
        var buffer = Encoding.ASCII.GetBytes("image");
        var exampleStream = new MemoryStream(buffer);
        return new FileInput("jpg", exampleStream);
    }

    public FileInput GetValidMediaFileInput()
    {
        var buffer = Encoding.ASCII.GetBytes("video");
        var exampleStream = new MemoryStream(buffer);
        return new FileInput("mp4", exampleStream);
    }
}