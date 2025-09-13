using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.VideoRepository;

[Trait("Integration/Infra.Data", "Video Repository - Repositories")]
public sealed class VideoRepositoryTest(VideoRepositoryTestFixture fixture) : IClassFixture<VideoRepositoryTestFixture>
{
    [Fact(DisplayName = nameof(Insert))]
    public async Task Insert()
    {
        var dbContext = fixture.CreateDbContext();
        var exampleVideo = fixture.GetExampleVideo();
        IVideoRepository videoRepository = new Repository.VideoRepository(dbContext);

        await videoRepository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = fixture.CreateDbContext(true);
        Video? dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo.Id.Should().Be(exampleVideo.Id);
        dbVideo.Title.Should().Be(exampleVideo.Title);
        dbVideo.Description.Should().Be(exampleVideo.Description);
        dbVideo.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        dbVideo.Opened.Should().Be(exampleVideo.Opened);
        dbVideo.Published.Should().Be(exampleVideo.Published);
        dbVideo.Duration.Should().Be(exampleVideo.Duration);
        dbVideo.CreatedAt.Should().BeCloseTo(exampleVideo.CreatedAt, TimeSpan.FromSeconds(1));
        dbVideo.Rating.Should().Be(exampleVideo.Rating);

        dbVideo.Thumb.Should().BeNull();
        dbVideo.ThumbHalf.Should().BeNull();
        dbVideo.Banner.Should().BeNull();
        dbVideo.Media.Should().BeNull();
        dbVideo.Trailer.Should().BeNull();

        dbVideo.Genres.Should().BeEmpty();
        dbVideo.Categories.Should().BeEmpty();
        dbVideo.CastMembers.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(InsertWithMediasAndImages))]
    public async Task InsertWithMediasAndImages()
    {
        var dbContext = fixture.CreateDbContext();
        var exampleVideo = fixture.GetValidVideoWithAllProperties();
        var videoRepository = new Repository.VideoRepository(dbContext);

        await videoRepository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos
            .Include(x => x.Media)
            .Include(x => x.Trailer)
            .FirstOrDefaultAsync(video => video.Id == exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo.Id.Should().Be(exampleVideo.Id);
        dbVideo.Thumb.Should().NotBeNull();
        dbVideo.Thumb!.Path.Should().Be(exampleVideo.Thumb!.Path);
        dbVideo.ThumbHalf.Should().NotBeNull();
        dbVideo.ThumbHalf!.Path.Should().Be(exampleVideo.ThumbHalf!.Path);
        dbVideo.Banner.Should().NotBeNull();
        dbVideo.Banner!.Path.Should().Be(exampleVideo.Banner!.Path);
        dbVideo.Media.Should().NotBeNull();
        dbVideo.Media!.FilePath.Should().Be(exampleVideo.Media!.FilePath);
        dbVideo.Media.EncodedPath.Should().Be(exampleVideo.Media.EncodedPath);
        dbVideo.Media.Status.Should().Be(exampleVideo.Media.Status);
        dbVideo.Trailer.Should().NotBeNull();
        dbVideo.Trailer!.FilePath.Should().Be(exampleVideo.Trailer!.FilePath);
        dbVideo.Trailer.EncodedPath.Should().Be(exampleVideo.Trailer.EncodedPath);
        dbVideo.Trailer.Status.Should().Be(exampleVideo.Trailer.Status);
    }

    [Fact(DisplayName = nameof(InsertWithRelations))]
    public async Task InsertWithRelations()
    {
        var dbContext = fixture.CreateDbContext();
        var exampleVideo = fixture.GetExampleVideo();
        var castMembers = fixture.GetRandomCastMembersList().ToList();
        castMembers.ForEach(castMember => exampleVideo.AddCastMember(castMember.Id));

        await dbContext.CastMembers.AddRangeAsync(castMembers);
        var categories = fixture.GetRandomCategoriesList().ToList();
        categories.ForEach(category => exampleVideo.AddCategory(category.Id));

        await dbContext.Categories.AddRangeAsync(categories);
        var genres = fixture.GetRandomGenresList().ToList();
        genres.ForEach(genre => exampleVideo.AddGenre(genre.Id));

        await dbContext.Genres.AddRangeAsync(genres);
        await dbContext.SaveChangesAsync();
        var videoRepository = new Repository.VideoRepository(dbContext);

        await videoRepository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();

        var dbVideosCategories = assertsDbContext.VideosCategories
            .Where(relation => relation.VideoId == exampleVideo.Id)
            .ToList();
        dbVideosCategories.Should().HaveCount(categories.Count);
        dbVideosCategories
            .Select(relation => relation.CategoryId)
            .Should()
            .BeEquivalentTo(categories.Select(category => category.Id));

        var dbVideosGenres = assertsDbContext.VideosGenres
            .Where(relation => relation.VideoId == exampleVideo.Id)
            .ToList();
        dbVideosGenres.Should().HaveCount(genres.Count);
        dbVideosGenres.Select(relation => relation.GenreId)
            .Should()
            .BeEquivalentTo(genres.Select(genre => genre.Id));

        var dbVideosCastMembers = assertsDbContext.VideosCastMembers
            .Where(relation => relation.VideoId == exampleVideo.Id)
            .ToList();
        dbVideosCastMembers.Should().HaveCount(castMembers.Count);
        dbVideosCastMembers
            .Select(relation => relation.CastMemberId)
            .Should()
            .BeEquivalentTo(castMembers.Select(castMember => castMember.Id));
    }

    [Fact(DisplayName = nameof(Update))]
    public async Task Update()
    {
        var dbContextArrange = fixture.CreateDbContext();
        var exampleVideo = fixture.GetExampleVideo();
        await dbContextArrange.AddAsync(exampleVideo);
        await dbContextArrange.SaveChangesAsync();
        var newValuesVideo = fixture.GetExampleVideo();
        var dbContextAct = fixture.CreateDbContext(true);
        var videoRepository = new Repository.VideoRepository(dbContextAct);

        exampleVideo.Update(
            newValuesVideo.Title,
            newValuesVideo.Description,
            newValuesVideo.YearLaunched,
            newValuesVideo.Duration,
            newValuesVideo.Opened,
            newValuesVideo.Published,
            newValuesVideo.Rating);
        await videoRepository.Update(exampleVideo, CancellationToken.None);
        await dbContextArrange.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo.Id.Should().Be(exampleVideo.Id);
        dbVideo.Title.Should().Be(exampleVideo.Title);
        dbVideo.Description.Should().Be(exampleVideo.Description);
        dbVideo.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        dbVideo.Opened.Should().Be(exampleVideo.Opened);
        dbVideo.Published.Should().Be(exampleVideo.Published);
        dbVideo.Duration.Should().Be(exampleVideo.Duration);
        dbVideo.Rating.Should().Be(exampleVideo.Rating);
        dbVideo.Thumb.Should().BeNull();
        dbVideo.ThumbHalf.Should().BeNull();
        dbVideo.Banner.Should().BeNull();
        dbVideo.Media.Should().BeNull();
        dbVideo.Trailer.Should().BeNull();
        dbVideo.Genres.Should().BeEmpty();
        dbVideo.Categories.Should().BeEmpty();
        dbVideo.CastMembers.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(UpdateEntitiesAndValueObjects))]
    public async Task UpdateEntitiesAndValueObjects()
    {
        var dbContextArrange = fixture.CreateDbContext();
        var exampleVideo = fixture.GetExampleVideo();
        exampleVideo.UpdateTrailer(fixture.GetValidMediaPath());
        await dbContextArrange.AddAsync(exampleVideo);
        await dbContextArrange.SaveChangesAsync();
        var updatedThumb = fixture.GetValidImagePath();
        var updatedThumbHalf = fixture.GetValidImagePath();
        var updatedBanner = fixture.GetValidImagePath();
        var updatedMedia = fixture.GetValidMediaPath();
        var updatedMediaEncoded = fixture.GetValidMediaPath();
        var updatedTrailer = fixture.GetValidMediaPath();
        var dbContextAct = fixture.CreateDbContext(true);
        var videoRepository = new Repository.VideoRepository(dbContextAct);
        var savedVideo = dbContextAct.Videos.Single(v => v.Id == exampleVideo.Id);

        savedVideo.UpdateThumb(updatedThumb);
        savedVideo.UpdateThumbHalf(updatedThumbHalf);
        savedVideo.UpdateBanner(updatedBanner);
        savedVideo.UpdateTrailer(updatedTrailer);
        savedVideo.UpdateMedia(updatedMedia);
        savedVideo.UpdateAsEncoded(updatedMediaEncoded);
        await videoRepository.Update(savedVideo, CancellationToken.None);
        await dbContextAct.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo.Thumb.Should().NotBeNull();
        dbVideo.ThumbHalf.Should().NotBeNull();
        dbVideo.Banner.Should().NotBeNull();
        dbVideo.Media.Should().NotBeNull();
        dbVideo.Trailer.Should().NotBeNull();
        dbVideo.Thumb.Path.Should().Be(updatedThumb);
        dbVideo.ThumbHalf.Path.Should().Be(updatedThumbHalf);
        dbVideo.Banner.Path.Should().Be(updatedBanner);
        dbVideo.Media!.FilePath.Should().Be(updatedMedia);
        dbVideo.Media.EncodedPath.Should().Be(updatedMediaEncoded);
        dbVideo.Media.Status.Should().Be(MediaStatus.Completed);
        dbVideo.Trailer!.FilePath.Should().Be(updatedTrailer);
    }

    [Fact(DisplayName = nameof(UpdateWithRelations))]
    public async Task UpdateWithRelations()
    {
        Guid id;
        var castMembers = fixture.GetRandomCastMembersList();
        var categories = fixture.GetRandomCategoriesList();
        var genres = fixture.GetRandomGenresList();
        await using (var dbContext = fixture.CreateDbContext())
        {
            var exampleVideo = fixture.GetExampleVideo();
            id = exampleVideo.Id;
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.CastMembers.AddRangeAsync(castMembers);
            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.Genres.AddRangeAsync(genres);
            await dbContext.SaveChangesAsync();
        }

        var actDbContext = fixture.CreateDbContext(true);
        var savedVideo = actDbContext.Videos
            .First(video => video.Id == id);
        var videoRepository = new Repository.VideoRepository(actDbContext);

        castMembers.ToList().ForEach(castMember
            => savedVideo.AddCastMember(castMember.Id));
        categories.ToList().ForEach(category
            => savedVideo.AddCategory(category.Id));
        genres.ToList().ForEach(genre
            => savedVideo.AddGenre(genre.Id));

        await videoRepository.Update(savedVideo, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(id);
        dbVideo.Should().NotBeNull();
        var dbVideosCategories = assertsDbContext.VideosCategories
            .Where(relation => relation.VideoId == id)
            .ToList();
        dbVideosCategories.Should().HaveCount(categories.Count);
        dbVideosCategories.Select(relation => relation.CategoryId).ToList()
            .Should().BeEquivalentTo(
                categories.Select(category => category.Id));
        var dbVideosGenres = assertsDbContext.VideosGenres
            .Where(relation => relation.VideoId == id)
            .ToList();
        dbVideosGenres.Should().HaveCount(genres.Count);
        dbVideosGenres.Select(relation => relation.GenreId).ToList()
            .Should().BeEquivalentTo(
                genres.Select(genre => genre.Id));
        var dbVideosCastMembers = assertsDbContext.VideosCastMembers
            .Where(relation => relation.VideoId == id)
            .ToList();
        dbVideosCastMembers.Should().HaveCount(castMembers.Count);
        dbVideosCastMembers.Select(relation => relation.CastMemberId).ToList()
            .Should().BeEquivalentTo(
                castMembers.Select(castMember => castMember.Id));
    }

    [Fact(DisplayName = nameof(Delete))]
    public async Task Delete()
    {
        Guid id;
        await using (var dbContext = fixture.CreateDbContext())
        {
            var exampleVideo = fixture.GetExampleVideo();
            id = exampleVideo.Id;
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.SaveChangesAsync();
        }

        var actDbContext = fixture.CreateDbContext(true);
        var savedVideo = actDbContext.Videos.First(video => video.Id == id);
        IVideoRepository videoRepository = new Repository.VideoRepository(actDbContext);

        await videoRepository.Delete(savedVideo, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(id);
        dbVideo.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DeleteWithAllPropertiesAndRelations))]
    public async Task DeleteWithAllPropertiesAndRelations()
    {
        Guid id;
        await using (var dbContext = fixture.CreateDbContext())
        {
            var exampleVideo = fixture.GetValidVideoWithAllProperties();
            id = exampleVideo.Id;
            var castMembers = fixture.GetRandomCastMembersList();
            var categories = fixture.GetRandomCategoriesList();
            var genres = fixture.GetRandomGenresList();
            castMembers.ToList().ForEach(castMember
                => dbContext.VideosCastMembers.Add(new(castMember.Id, id)));
            categories.ToList().ForEach(category
                => dbContext.VideosCategories.Add(new(category.Id, id)));
            genres.ToList().ForEach(genre
                => dbContext.VideosGenres.Add(new(genre.Id, id)));
            await dbContext.CastMembers.AddRangeAsync(castMembers);
            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.Genres.AddRangeAsync(genres);
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.SaveChangesAsync();
        }

        var actDbContext = fixture.CreateDbContext(true);
        var savedVideo = actDbContext.Videos.First(video => video.Id == id);
        var videoRepository = new Repository.VideoRepository(actDbContext);

        await videoRepository.Delete(savedVideo, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(id);
        dbVideo.Should().BeNull();

        assertsDbContext.VideosCategories
            .Where(relation => relation.VideoId == id)
            .ToList().Count.Should().Be(0);
        assertsDbContext.VideosGenres
            .Where(relation => relation.VideoId == id)
            .ToList().Count.Should().Be(0);
        assertsDbContext.VideosCastMembers
            .Where(relation => relation.VideoId == id)
            .ToList().Count.Should().Be(0);
        assertsDbContext.Set<Media>().Count().Should().Be(0);
    }

    [Fact(DisplayName = nameof(Get))]
    public async Task Get()
    {
        var exampleVideo = fixture.GetExampleVideo();
        await using (var dbContext = fixture.CreateDbContext())
        {
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.SaveChangesAsync();
        }

        var videoRepository = new Repository.VideoRepository(fixture.CreateDbContext(true));

        var video = await videoRepository.Get(exampleVideo.Id, CancellationToken.None);

        video.Id.Should().Be(exampleVideo.Id);
        video.Title.Should().Be(exampleVideo.Title);
        video.Description.Should().Be(exampleVideo.Description);
        video.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        video.Opened.Should().Be(exampleVideo.Opened);
        video.Published.Should().Be(exampleVideo.Published);
        video.Duration.Should().Be(exampleVideo.Duration);
        video.CreatedAt.Should().BeCloseTo(exampleVideo.CreatedAt, TimeSpan.FromSeconds(1));
        video.Rating.Should().Be(exampleVideo.Rating);
        video.Should().NotBeNull();
        video.ThumbHalf.Should().BeNull();
        video.Thumb.Should().BeNull();
        video.Banner.Should().BeNull();
        video.Media.Should().BeNull();
        video.Trailer.Should().BeNull();
    }

    [Fact(DisplayName = nameof(GetWithAllProperties))]
    public async Task GetWithAllProperties()
    {
        Guid id;
        var exampleVideo = fixture.GetValidVideoWithAllProperties();
        await using (var dbContext = fixture.CreateDbContext())
        {
            id = exampleVideo.Id;
            var castMembers = fixture.GetRandomCastMembersList();
            var categories = fixture.GetRandomCategoriesList();
            var genres = fixture.GetRandomGenresList();
            castMembers.ToList().ForEach(castMember =>
            {
                exampleVideo.AddCastMember(castMember.Id);
                dbContext.VideosCastMembers.Add(new VideosCastMembers(castMember.Id, id));
            });
            categories.ToList().ForEach(category =>
            {
                exampleVideo.AddCategory(category.Id);
                dbContext.VideosCategories.Add(new VideosCategories(category.Id, id));
            });
            genres.ToList().ForEach(genre =>
            {
                exampleVideo.AddGenre(genre.Id);
                dbContext.VideosGenres.Add(new VideosGenres(genre.Id, id));
            });
            await dbContext.CastMembers.AddRangeAsync(castMembers);
            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.Genres.AddRangeAsync(genres);
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.SaveChangesAsync();
        }

        IVideoRepository videoRepository =
            new Repository.VideoRepository(fixture.CreateDbContext(true));

        var video = await videoRepository.Get(id, CancellationToken.None);

        video.Should().NotBeNull();
        video.Id.Should().Be(exampleVideo.Id);
        video.Title.Should().Be(exampleVideo.Title);
        video.Description.Should().Be(exampleVideo.Description);
        video.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        video.Opened.Should().Be(exampleVideo.Opened);
        video.Published.Should().Be(exampleVideo.Published);
        video.Duration.Should().Be(exampleVideo.Duration);
        video.CreatedAt.Should().BeCloseTo(exampleVideo.CreatedAt, TimeSpan.FromSeconds(1));
        video.Rating.Should().Be(exampleVideo.Rating);
        video.ThumbHalf.Should().NotBeNull();
        video.Thumb.Should().NotBeNull();
        video.Banner.Should().NotBeNull();
        video.Media.Should().NotBeNull();
        video.Trailer.Should().NotBeNull();
        video.ThumbHalf!.Path.Should().Be(exampleVideo.ThumbHalf!.Path);
        video.Thumb!.Path.Should().Be(exampleVideo.Thumb!.Path);
        video.Banner!.Path.Should().Be(exampleVideo.Banner!.Path);
        video.Media!.FilePath.Should().Be(exampleVideo.Media!.FilePath);
        video.Media.EncodedPath.Should().Be(exampleVideo.Media.EncodedPath);
        video.Media.Status.Should().Be(exampleVideo.Media.Status);
        video.Trailer!.FilePath.Should().Be(exampleVideo.Trailer!.FilePath);
        video.Trailer.EncodedPath.Should().Be(exampleVideo.Trailer.EncodedPath);
        video.Trailer.Status.Should().Be(exampleVideo.Trailer.Status);
        video.Genres.Should().BeEquivalentTo(exampleVideo.Genres);
        video.Categories.Should().BeEquivalentTo(exampleVideo.Categories);
        video.CastMembers.Should().BeEquivalentTo(exampleVideo.CastMembers);
    }

    [Fact(DisplayName = nameof(GetThrowIfNotFind))]
    public async Task GetThrowIfNotFind()
    {
        var id = Guid.NewGuid();
        IVideoRepository videoRepository =
            new Repository.VideoRepository(fixture.CreateDbContext());

        var action = () => videoRepository.Get(id, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Video '{id}' not found.");
    }

    [Fact(DisplayName = nameof(Search))]
    public async Task Search()
    {
        var exampleVideosList = fixture.GetExampleVideosList();
        await using (var arrangeDbContext = fixture.CreateDbContext())
        {
            await arrangeDbContext.Videos.AddRangeAsync(exampleVideosList);
            await arrangeDbContext.SaveChangesAsync();
        }

        var actDbContext = fixture.CreateDbContext(true);
        var videoRepository = new Repository.VideoRepository(actDbContext);
        var searchInput = new SearchInput(1, 20, "", "", default);

        var result = await videoRepository.Search(searchInput, CancellationToken.None);

        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Total.Should().Be(exampleVideosList.Count);
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(exampleVideosList.Count);
        result.Items.ToList().ForEach(resultItem =>
        {
            var exampleVideo = exampleVideosList
                .FirstOrDefault(x => x.Id == resultItem.Id);
            exampleVideosList.Should().NotBeNull();
            resultItem.Id.Should().Be(exampleVideo!.Id);
            resultItem.Title.Should().Be(exampleVideo.Title);
            resultItem.Description.Should().Be(exampleVideo.Description);
            resultItem.YearLaunched.Should().Be(exampleVideo.YearLaunched);
            resultItem.Opened.Should().Be(exampleVideo.Opened);
            resultItem.Published.Should().Be(exampleVideo.Published);
            resultItem.Duration.Should().Be(exampleVideo.Duration);
            resultItem.CreatedAt.Should().BeCloseTo(exampleVideo.CreatedAt, TimeSpan.FromSeconds(1));
            resultItem.Rating.Should().Be(exampleVideo.Rating);
            resultItem.Thumb.Should().BeNull();
            resultItem.ThumbHalf.Should().BeNull();
            resultItem.Banner.Should().BeNull();
            resultItem.Media.Should().BeNull();
            resultItem.Trailer.Should().BeNull();
            resultItem.Genres.Should().BeEmpty();
            resultItem.Categories.Should().BeEmpty();
            resultItem.CastMembers.Should().BeEmpty();
        });
    }

    [Fact(DisplayName = nameof(SearchReturnsAllRelations))]
    public async Task SearchReturnsAllRelations()
    {
        var exampleVideosList = fixture.GetExampleVideosList();
        await using (var arrangeDbContext = fixture.CreateDbContext())
        {
            foreach (var exampleVideo in exampleVideosList)
            {
                var castMembers = fixture.GetRandomCastMembersList();
                var categories = fixture.GetRandomCategoriesList();
                var genres = fixture.GetRandomGenresList();
                castMembers.ToList().ForEach(castMember =>
                {
                    exampleVideo.AddCastMember(castMember.Id);
                    arrangeDbContext.VideosCastMembers.Add(new(castMember.Id, exampleVideo.Id));
                });
                categories.ToList().ForEach(category =>
                {
                    exampleVideo.AddCategory(category.Id);
                    arrangeDbContext.VideosCategories.Add(new(category.Id, exampleVideo.Id));
                });
                genres.ToList().ForEach(genre =>
                {
                    exampleVideo.AddGenre(genre.Id);
                    arrangeDbContext.VideosGenres.Add(new(genre.Id, exampleVideo.Id));
                });
                await arrangeDbContext.CastMembers.AddRangeAsync(castMembers);
                await arrangeDbContext.Categories.AddRangeAsync(categories);
                await arrangeDbContext.Genres.AddRangeAsync(genres);
            }

            await arrangeDbContext.Videos.AddRangeAsync(exampleVideosList);
            await arrangeDbContext.SaveChangesAsync();
        }

        var actDbContext = fixture.CreateDbContext(true);
        var videoRepository = new Repository.VideoRepository(actDbContext);
        var searchInput = new SearchInput(1, 20, "", "", default);

        var result = await videoRepository.Search(searchInput, CancellationToken.None);

        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Total.Should().Be(exampleVideosList.Count);
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(exampleVideosList.Count);
        result.Items.ToList().ForEach(resultItem =>
        {
            var exampleVideo = exampleVideosList
                .FirstOrDefault(x => x.Id == resultItem.Id);
            exampleVideo.Should().NotBeNull();
            resultItem.Genres.Should().BeEquivalentTo(exampleVideo.Genres);
            resultItem.Categories.Should().BeEquivalentTo(exampleVideo.Categories);
            resultItem.CastMembers.Should().BeEquivalentTo(exampleVideo.CastMembers);
        });
    }


    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenEmpty))]
    public async Task SearchReturnsEmptyWhenEmpty()
    {
        var actDbContext = fixture.CreateDbContext();
        var videoRepository = new Repository.VideoRepository(actDbContext);
        var searchInput = new SearchInput(1, 20, "", "", default);

        var result = await videoRepository.Search(searchInput, CancellationToken.None);

        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Total.Should().Be(0);
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(0);
    }


    [Theory(DisplayName = nameof(SearchPagination))]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchPagination(
        int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {
        var exampleVideosList = fixture.GetExampleVideosList(quantityToGenerate);
        await using (var arrangeDbContext = fixture.CreateDbContext())
        {
            await arrangeDbContext.Videos.AddRangeAsync(exampleVideosList);
            await arrangeDbContext.SaveChangesAsync();
        }

        var actDbContext = fixture.CreateDbContext(true);
        var videoRepository = new Repository.VideoRepository(actDbContext);
        var searchInput = new SearchInput(page, perPage, "", "", default);

        var result = await videoRepository.Search(searchInput, CancellationToken.None);

        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Total.Should().Be(exampleVideosList.Count);
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(expectedQuantityItems);
        result.Items.ToList().ForEach(resultItem =>
        {
            var exampleVideo = exampleVideosList
                .FirstOrDefault(x => x.Id == resultItem.Id);
            exampleVideosList.Should().NotBeNull();
            resultItem.Id.Should().Be(exampleVideo!.Id);
            resultItem.Title.Should().Be(exampleVideo.Title);
            resultItem.Description.Should().Be(exampleVideo.Description);
        });
    }


    [Theory(DisplayName = nameof(SearchByTitle))]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    [InlineData("", 1, 5, 5, 9)]
    [InlineData("test-no-items", 1, 5, 0, 0)]
    public async Task SearchByTitle(
        string search,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems
    )
    {
        var exampleVideosList = fixture.GetExampleVideosListByTitles(
        [
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Space",
            "Sci-fi Robots",
            "Sci-fi Future"
        ]);
        await using (var arrangeDbContext = fixture.CreateDbContext())
        {
            await arrangeDbContext.Videos.AddRangeAsync(exampleVideosList);
            await arrangeDbContext.SaveChangesAsync();
        }

        var actDbContext = fixture.CreateDbContext(true);
        var videoRepository = new Repository.VideoRepository(actDbContext);
        var searchInput = new SearchInput(page, perPage, search, "", default);

        var result = await videoRepository.Search(searchInput, CancellationToken.None);

        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Total.Should().Be(expectedQuantityTotalItems);
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(expectedQuantityItemsReturned);
        result.Items.ToList().ForEach(resultItem =>
        {
            var exampleVideo = exampleVideosList
                .FirstOrDefault(x => x.Id == resultItem.Id);
            exampleVideosList.Should().NotBeNull();
            resultItem.Id.Should().Be(exampleVideo!.Id);
            resultItem.Title.Should().Be(exampleVideo.Title);
            resultItem.Description.Should().Be(exampleVideo.Description);
        });
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [InlineData("title", "asc")]
    [InlineData("title", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    public async Task SearchOrdered(string orderBy, string order)
    {
        var exampleVideosList = fixture.GetExampleVideosList();
        await using (var arrangeDbContext = fixture.CreateDbContext())
        {
            await arrangeDbContext.Videos.AddRangeAsync(exampleVideosList);
            await arrangeDbContext.SaveChangesAsync();
        }

        var actDbContext = fixture.CreateDbContext(true);
        var videoRepository = new Repository.VideoRepository(actDbContext);
        var searchOrder = order.Equals("asc", StringComparison.CurrentCultureIgnoreCase)
            ? SearchOrder.Asc
            : SearchOrder.Desc;
        var searchInput = new SearchInput(1, 20, "", orderBy, searchOrder);

        var result = await videoRepository.Search(searchInput, CancellationToken.None);

        var orderedList = fixture.CloneListOrdered(exampleVideosList, searchInput);
        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Total.Should().Be(exampleVideosList.Count);
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(exampleVideosList.Count);
        for (int index = 0; index < orderedList.Count; index++)
        {
            var resultItem = result.Items[index];
            var exampleVideo = orderedList[index];
            resultItem.Id.Should().Be(exampleVideo.Id);
            resultItem.Title.Should().Be(exampleVideo.Title);
            resultItem.Description.Should().Be(exampleVideo.Description);
        }
    }
}