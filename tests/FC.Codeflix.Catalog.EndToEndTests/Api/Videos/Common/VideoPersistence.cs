using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;

using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Videos.Common;

public sealed class VideoPersistence(CodeflixCatalogDbContext context)
{
    public async Task<Video?> GetById(Guid id)
        => await context.Videos.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<List<VideosCastMembers>> GetVideosCastMembers(Guid videoId)
        => await context.VideosCastMembers.AsNoTracking()
            .Where(relation => relation.VideoId == videoId)
            .ToListAsync();

    public async Task<List<VideosGenres>> GetVideosGenres(Guid videoId)
        => await context.VideosGenres.AsNoTracking()
            .Where(relation => relation.VideoId == videoId)
            .ToListAsync();

    public async Task<List<VideosCategories>> GetVideosCategories(Guid videoId)
        => await context.VideosCategories.AsNoTracking()
            .Where(relation => relation.VideoId == videoId)
            .ToListAsync();

    public async Task<int> GetMediaCount() => await context.Set<Media>().CountAsync();

    public async Task InsertList(List<Video> videos)
    {
        await context.Videos.AddRangeAsync(videos);
        foreach (var video in videos)
        {
            var videosCategories = video.Categories?.Select(categoryId => new VideosCategories(categoryId, video.Id))
                .ToList();
            if (videosCategories != null && videosCategories.Count != 0)
            {
                await context.VideosCategories.AddRangeAsync(videosCategories);
            }

            var videosGenres = video.Genres?.Select(genreId => new VideosGenres(genreId, video.Id)).ToList();
            if (videosGenres != null && videosGenres.Count != 0)
            {
                await context.VideosGenres.AddRangeAsync(videosGenres);
            }

            var videosCastMembers = video.CastMembers?
                .Select(castMemberId => new VideosCastMembers(castMemberId, video.Id)).ToList();
            if (videosCastMembers != null && videosCastMembers.Count != 0)
            {
                await context.VideosCastMembers.AddRangeAsync(videosCastMembers);
            }
        }

        await context.SaveChangesAsync();
    }
}