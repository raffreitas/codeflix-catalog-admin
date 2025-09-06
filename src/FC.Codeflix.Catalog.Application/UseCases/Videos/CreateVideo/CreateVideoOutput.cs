using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.CreateVideo;

public sealed record CreateVideoOutput(
    Guid Id,
    string Title,
    string Description,
    int YearLaunched,
    int Duration,
    bool Opened,
    bool Published,
    Rating Rating,
    DateTime CreatedAt,
    IReadOnlyCollection<Guid> CategoriesIds,
    IReadOnlyCollection<Guid> GenresIds,
    IReadOnlyCollection<Guid> CastMembersIds,
    string? Thumb,
    string? Banner,
    string? ThumbHalf,
    string? Media,
    string? Trailer
)
{
    public static CreateVideoOutput FromVideo(Video video) => new(
        Id: video.Id,
        Title: video.Title,
        Description: video.Description,
        YearLaunched: video.YearLaunched,
        Duration: video.Duration,
        Opened: video.Opened,
        Published: video.Published,
        Rating: video.Rating,
        CreatedAt: video.CreatedAt,
        CategoriesIds: video.Categories,
        GenresIds: video.Genres,
        CastMembersIds: video.CastMembers,
        Thumb: video.Thumb?.Path,
        Banner: video.Banner?.Path,
        ThumbHalf: video.ThumbHalf?.Path,
        Media: video.Media?.FilePath,
        Trailer: video.Trailer?.FilePath
    );
};