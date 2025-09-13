using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;

using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

public sealed class VideoRepository(CodeflixCatalogDbContext context) : IVideoRepository
{
    private DbSet<Video> Videos => context.Set<Video>();
    private DbSet<Media> Medias => context.Set<Media>();

    private DbSet<VideosCategories> VideosCategories => context.Set<VideosCategories>();
    private DbSet<VideosGenres> VideosGenres => context.Set<VideosGenres>();
    private DbSet<VideosCastMembers> VideosCastMembers => context.Set<VideosCastMembers>();

    public async Task Insert(Video video, CancellationToken cancellationToken)
    {
        await Videos.AddAsync(video, cancellationToken);

        if (video.Categories.Count > 0)
        {
            var relations = video.Categories
                .Select(categoryId => new VideosCategories(
                    categoryId,
                    video.Id
                ));
            await VideosCategories.AddRangeAsync(relations, cancellationToken);
        }

        if (video.Genres.Count > 0)
        {
            var relations = video.Genres
                .Select(genreId => new VideosGenres(
                    genreId,
                    video.Id
                ));
            await VideosGenres.AddRangeAsync(relations, cancellationToken);
        }

        if (video.CastMembers.Count > 0)
        {
            var relations = video.CastMembers
                .Select(castMemberId => new VideosCastMembers(
                    castMemberId,
                    video.Id
                ));
            await VideosCastMembers.AddRangeAsync(relations, cancellationToken);
        }
    }

    public async Task Update(Video video, CancellationToken cancellationToken)
    {
        Videos.Update(video);

        VideosCategories.RemoveRange(VideosCategories.Where(x => x.VideoId == video.Id));
        VideosCastMembers.RemoveRange(VideosCastMembers.Where(x => x.VideoId == video.Id));
        VideosGenres.RemoveRange(VideosGenres.Where(x => x.VideoId == video.Id));

        if (video.Categories.Count > 0)
        {
            var relations = video.Categories
                .Select(categoryId => new VideosCategories(
                    categoryId,
                    video.Id
                ));
            await VideosCategories.AddRangeAsync(relations, cancellationToken);
        }

        if (video.Genres.Count > 0)
        {
            var relations = video.Genres
                .Select(genreId => new VideosGenres(
                    genreId,
                    video.Id
                ));
            await VideosGenres.AddRangeAsync(relations, cancellationToken);
        }

        if (video.CastMembers.Count > 0)
        {
            var relations = video.CastMembers
                .Select(castMemberId => new VideosCastMembers(
                    castMemberId,
                    video.Id
                ));
            await VideosCastMembers.AddRangeAsync(relations, cancellationToken);
        }

        DeleteOrphanMedias(video);
    }

    private void DeleteOrphanMedias(Video video)
    {
        if (context.Entry(video).Reference(v => v.Trailer).IsModified)
        {
            var oldTrailerId = context.Entry(video).OriginalValues.GetValue<Guid?>($"{nameof(Video.Trailer)}Id");
            if (oldTrailerId != null && oldTrailerId != video.Trailer?.Id)
            {
                var oldTrailer = Medias.Find(oldTrailerId);
                Medias.Remove(oldTrailer!);
            }
        }

        if (context.Entry(video).Reference(v => v.Media).IsModified)
        {
            var oldMediaId = context.Entry(video).OriginalValues.GetValue<Guid?>($"{nameof(Video.Media)}Id");
            if (oldMediaId != null && oldMediaId != video.Media?.Id)
            {
                var oldMedia = Medias.Find(oldMediaId);
                Medias.Remove(oldMedia!);
            }
        }
    }

    public Task Delete(Video video, CancellationToken cancellationToken)
    {
        VideosCategories.RemoveRange(VideosCategories.Where(x => x.VideoId == video.Id));
        VideosCastMembers.RemoveRange(VideosCastMembers.Where(x => x.VideoId == video.Id));
        VideosGenres.RemoveRange(VideosGenres.Where(x => x.VideoId == video.Id));

        if (video.Trailer is not null) Medias.Remove(video.Trailer);
        if (video.Media is not null) Medias.Remove(video.Media);

        Videos.Remove(video);
        return Task.CompletedTask;
    }

    public async Task<Video> Get(Guid id, CancellationToken cancellationToken)
    {
        var video = await Videos.FirstOrDefaultAsync(video => video.Id == id, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(video, $"Video '{id}' not found.");

        var categoryIds = await VideosCategories
            .AsNoTracking()
            .Where(x => x.VideoId == video!.Id)
            .Select(x => x.CategoryId)
            .ToListAsync(cancellationToken);
        categoryIds.ForEach(video!.AddCategory);

        var genresIds = await VideosGenres
            .AsNoTracking()
            .Where(x => x.VideoId == video.Id)
            .Select(x => x.GenreId)
            .ToListAsync(cancellationToken);
        genresIds.ForEach(video.AddGenre);

        var castMembersIds = await VideosCastMembers
            .AsNoTracking()
            .Where(x => x.VideoId == video.Id)
            .Select(x => x.CastMemberId)
            .ToListAsync(cancellationToken);
        castMembersIds.ForEach(video.AddCastMember);

        return video;
    }

    public async Task<SearchOutput<Video>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;
        var query = Videos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(input.Search))
            query = query.Where(video => video.Title.Contains(input.Search));

        query = InsertOrderBy(input, query);

        var count = query.Count();
        var items = await query.Skip(toSkip).Take(input.PerPage)
            .ToListAsync(cancellationToken);

        var videosIds = items.Select(video => video.Id).ToList();
        await AddCategoriesToVideos(items, videosIds);
        await AddGenresToVideos(items, videosIds);
        await AddCastMembersToVideos(items, videosIds);

        return new(
            input.Page,
            input.PerPage,
            count,
            items);
    }

    private async Task AddCategoriesToVideos(List<Video> items, List<Guid> videosIds)
    {
        var categoriesRelations = await VideosCategories
            .AsNoTracking()
            .Where(relation => videosIds.Contains(relation.VideoId))
            .ToListAsync();
        var relationsWithCategoriesByVideoId =
            categoriesRelations.GroupBy(x => x.VideoId).ToList();
        relationsWithCategoriesByVideoId.ForEach(relationGroup =>
        {
            var video = items.Find(video => video.Id == relationGroup.Key);
            if (video is null) return;
            relationGroup.ToList()
                .ForEach(relation => video.AddCategory(relation.CategoryId));
        });
    }

    private async Task AddGenresToVideos(List<Video> items, List<Guid> videosIds)
    {
        var genresRelations = await VideosGenres
            .AsNoTracking()
            .Where(relation => videosIds.Contains(relation.VideoId))
            .ToListAsync();
        var relationsWithGenresByVideoId =
            genresRelations.GroupBy(x => x.VideoId).ToList();
        relationsWithGenresByVideoId.ForEach(relationGroup =>
        {
            var video = items.Find(video => video.Id == relationGroup.Key);
            if (video is null) return;
            relationGroup.ToList()
                .ForEach(relation => video.AddGenre(relation.GenreId));
        });
    }

    private async Task AddCastMembersToVideos(List<Video> items, List<Guid> videosIds)
    {
        var castMembersRelations = await VideosCastMembers
            .AsNoTracking()
            .Where(relation => videosIds.Contains(relation.VideoId))
            .ToListAsync();
        var relationsWithCastMembersByVideoId =
            castMembersRelations.GroupBy(x => x.VideoId).ToList();
        relationsWithCastMembersByVideoId.ForEach(relationGroup =>
        {
            var video = items.Find(video => video.Id == relationGroup.Key);
            if (video is null) return;
            relationGroup.ToList()
                .ForEach(relation => video.AddCastMember(relation.CastMemberId));
        });
    }

    private static IQueryable<Video> InsertOrderBy(SearchInput input, IQueryable<Video> query) => input switch
    {
        { Order: SearchOrder.Asc } when input.OrderBy.ToLower() is "title" => query.OrderBy(video => video.Title)
            .ThenBy(video => video.Id),
        { Order: SearchOrder.Desc } when input.OrderBy.ToLower() is "title" => query
            .OrderByDescending(video => video.Title).ThenByDescending(video => video.Id),
        { Order: SearchOrder.Asc } when input.OrderBy.ToLower() is "id" => query.OrderBy(video => video.Id),
        { Order: SearchOrder.Desc } when input.OrderBy.ToLower() is "id" => query.OrderByDescending(video => video.Id),
        { Order: SearchOrder.Asc } when input.OrderBy.ToLower() is "createdat" =>
            query.OrderBy(video => video.CreatedAt),
        { Order: SearchOrder.Desc } when input.OrderBy.ToLower() is "createdat" => query.OrderByDescending(video =>
            video.CreatedAt),
        _ => query.OrderBy(video => video.Title).ThenBy(video => video.Id)
    };
}