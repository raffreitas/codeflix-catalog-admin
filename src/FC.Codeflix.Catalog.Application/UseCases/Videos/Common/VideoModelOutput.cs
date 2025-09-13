using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Extensions;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.Common;

public sealed record VideoModelOutput(
    Guid Id,
    string Title,
    string Description,
    int YearLaunched,
    int Duration,
    bool Opened,
    bool Published,
    string Rating,
    DateTime CreatedAt,
    IReadOnlyCollection<VideoModelOutputRelatedAggregate> Categories,
    IReadOnlyCollection<VideoModelOutputRelatedAggregate> Genres,
    IReadOnlyCollection<VideoModelOutputRelatedAggregate> CastMembers,
    string? ThumbFileUrl,
    string? BannerFileUrl,
    string? ThumbHalfFileUrl,
    string? VideoFileUrl,
    string? TrailerFileUrl
)
{
    public static VideoModelOutput FromVideo(
        Video video,
        IReadOnlyList<Category>? categories = null,
        IReadOnlyCollection<Genre>? genres = null
    ) => new(
        Id: video.Id,
        Title: video.Title,
        Description: video.Description,
        YearLaunched: video.YearLaunched,
        Duration: video.Duration,
        Opened: video.Opened,
        Published: video.Published,
        Rating: video.Rating.ToStringSignal(),
        CreatedAt: video.CreatedAt,
        Categories: video.Categories.Select(id => new VideoModelOutputRelatedAggregate(
            id,
            categories?.FirstOrDefault(category => category.Id == id)?.Name
        )).ToList(),
        Genres: video.Genres.Select(id => new VideoModelOutputRelatedAggregate(
            id,
            genres?.FirstOrDefault(genre => genre.Id == id)?.Name
        )).ToList(),
        CastMembers: video.CastMembers.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList(),
        ThumbFileUrl: video.Thumb?.Path,
        BannerFileUrl: video.Banner?.Path,
        ThumbHalfFileUrl: video.ThumbHalf?.Path,
        VideoFileUrl: video.Media?.FilePath,
        TrailerFileUrl: video.Trailer?.FilePath
    );
}

public record VideoModelOutputRelatedAggregate(Guid Id, string? Name = null);