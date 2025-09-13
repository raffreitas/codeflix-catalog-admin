using System.Text;
using System.Text.Json;

using FC.Codeflix.Catalog.Api.Models.Videos;
using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Events;
using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMembers.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.Genres.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Videos.Common;

public sealed class VideoBaseFixture : GenreBaseFixture
{
    public readonly VideoPersistence VideoPersistence;
    public readonly CastMemberPersistence CastMemberPersistence;
    private const string VideoCreatedQueue = "video.created.queue";
    private const string RoutingKey = "video.created";

    public VideoBaseFixture()
    {
        VideoPersistence = new VideoPersistence(DbContext);
        CastMemberPersistence = new CastMemberPersistence(DbContext);
    }

    public void SetupRabbitMQ()
    {
        var channel = WebAppFactory.RabbitMQChannel;
        var exchange = WebAppFactory.RabbitMQConfiguration.Exchange;
        channel.ExchangeDeclare(exchange, "direct", true, false, null);
        channel.QueueDeclare(VideoCreatedQueue, true, false, false, null);
        channel.QueueBind(VideoCreatedQueue, exchange, RoutingKey, null);
    }

    public void TearDownRabbitMQ()
    {
        var channel = WebAppFactory.RabbitMQChannel;
        var exchange = WebAppFactory.RabbitMQConfiguration.Exchange;
        channel.QueueUnbind(VideoCreatedQueue, exchange, RoutingKey, null);
        channel.QueueDelete(VideoCreatedQueue, false, false);
        channel.ExchangeDelete(exchange, false);
    }

    public (VideoUploadedEvent?, uint) ReadMessageFromRabbitMQ()
    {
        var consumingResult = WebAppFactory.RabbitMQChannel.BasicGet(VideoCreatedQueue, true);
        var rawMessage = consumingResult.Body.ToArray();
        var stringMessage = Encoding.UTF8.GetString(rawMessage);
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower, };
        var @event = JsonSerializer.Deserialize<VideoUploadedEvent>(
            stringMessage, jsonOptions);
        return (@event, consumingResult.MessageCount);
    }

    #region Video

    public CreateVideoApiInput GetBasicCreateVideoInput()
    {
        return new CreateVideoApiInput
        {
            Description = GetValidDescription(),
            Title = GetValidTitle(),
            YearLaunched = GetValidYearLaunched(),
            Opened = GetRandomBoolean(),
            Published = GetRandomBoolean(),
            Duration = GetValidDuration(),
            Rating = GetRandomRating().ToStringSignal()
        };
    }

    public Video GetValidVideoWithAllProperties(string? title = null)
    {
        var video = new Video(
            title ?? GetValidTitle(),
            GetValidDescription(),
            GetValidYearLaunched(),
            GetValidDuration(),
            GetRandomBoolean(),
            GetRandomBoolean(),
            GetRandomRating()
        );

        video.UpdateBanner(GetValidImagePath());
        video.UpdateThumb(GetValidImagePath());
        video.UpdateThumbHalf(GetValidImagePath());

        video.UpdateMedia(GetValidMediaPath());
        video.UpdateTrailer(GetValidMediaPath());

        return video;
    }

    public List<Video> GetVideoCollection(int count = 10)
        => Enumerable
            .Range(1, count)
            .Select(_ =>
            {
                Thread.Sleep(1);
                return GetValidVideoWithAllProperties();
            }).ToList();

    public List<Video> GetVideoCollection(IEnumerable<string> titles)
        => titles
            .Select(title =>
            {
                Thread.Sleep(1);
                return GetValidVideoWithAllProperties(title);
            }).ToList();

    public IEnumerable<Video> CloneVideosOrdered(List<Video> videos, string orderBy, SearchOrder searchOrder)
    {
        var clone = new List<Video>(videos);
        return (orderBy.ToLower(), searchOrder) switch
        {
            ("title", SearchOrder.Asc) => clone.OrderBy(x => x.Title)
                .ThenBy(x => x.Id),
            ("title", SearchOrder.Desc) => clone.OrderByDescending(x => x.Title)
                .ThenByDescending(x => x.Id),
            ("id", SearchOrder.Asc) => clone.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => clone.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => clone.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => clone
                .OrderByDescending(x => x.CreatedAt),
            _ => clone.OrderBy(x => x.Title).ThenBy(x => x.Id)
        };
    }

    public Rating GetRandomRating()
    {
        var enumValue = Enum.GetValues<Rating>();
        var random = new Random();
        return enumValue[random.Next(enumValue.Length)];
    }

    public string GetValidTitle() => Faker.Lorem.Letter(100);

    public string GetValidDescription() => Faker.Commerce.ProductDescription();

    public string GetTooLongDescription() => Faker.Lorem.Letter(4001);

    public int GetValidYearLaunched() => Faker.Date.BetweenDateOnly(
        new DateOnly(1960, 1, 1),
        new DateOnly(2022, 1, 1)
    ).Year;

    public int GetValidDuration() => (new Random()).Next(100, 300);

    public string GetTooLongTitle() => Faker.Lorem.Letter(400);

    public string GetValidImagePath() => Faker.Image.PlaceImgUrl();

    public string GetValidMediaPath()
    {
        var exampleMedias = new[]
        {
            "https://www.googlestorage.com/file-example.mp4",
            "https://www.storage.com/another-example-of-video.mp4", "https://www.S3.com.br/example.mp4",
            "https://www.glg.io/file.mp4"
        };
        var random = new Random();
        return exampleMedias[random.Next(exampleMedias.Length)];
    }

    public FileInput GetValidImageFileInput()
    {
        var exampleStream = new MemoryStream(Encoding.ASCII.GetBytes("test"));
        var fileInput = new FileInput("jpg", exampleStream, "image/jpeg");
        return fileInput;
    }

    public FileInput GetValidMediaFileInput()
    {
        var exampleStream = new MemoryStream(Encoding.ASCII.GetBytes("test"));
        var fileInput = new FileInput("mp4", exampleStream, "video/mp4");
        return fileInput;
    }

    public Media GetValidMedia() => new(GetValidMediaPath());

    #endregion

    #region CastMember

    public CastMember GetExampleCastMember() => new(GetValidName(), GetRandomCastMemberType());
    public string GetValidName() => Faker.Name.FullName();
    public CastMemberType GetRandomCastMemberType() => (CastMemberType)(new Random()).Next(1, 2);

    public List<CastMember> GetExampleCastMembersList(int quantity = 10)
        => Enumerable
            .Range(1, quantity)
            .Select(_ =>
            {
                Thread.Sleep(1);
                return GetExampleCastMember();
            }).ToList();

    #endregion
}