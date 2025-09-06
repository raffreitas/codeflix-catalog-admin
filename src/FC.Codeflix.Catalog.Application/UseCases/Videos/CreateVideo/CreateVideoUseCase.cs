using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.Domain.Validations;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.CreateVideo;

public sealed class CreateVideoUseCase(
    IVideoRepository videoRepository,
    ICategoryRepository categoryRepository,
    IGenreRepository genreRepository,
    ICastMemberRepository castMemberRepository,
    IUnitOfWork uow,
    IStorageService storageService
) : ICreateVideoUseCase
{
    public async Task<CreateVideoOutput> Handle(CreateVideoInput request, CancellationToken cancellationToken)
    {
        var video = new Video(
            title: request.Title,
            description: request.Description,
            yearLaunched: request.YearLaunched,
            duration: request.Duration,
            opened: request.Opened,
            published: request.Published,
            rating: request.Rating
        );

        var validationHandler = new NotificationValidationHandler();
        video.Validate(validationHandler);
        if (validationHandler.HasErrors())
            throw new EntityValidationException("There are validation errors", validationHandler.Errors);

        await ValidateAndAddRelations(request, video, cancellationToken);

        try
        {
            await UploadImagesMedia(request, video, cancellationToken);

            await videoRepository.Insert(video, cancellationToken);
            await uow.Commit(cancellationToken);

            return CreateVideoOutput.FromVideo(video);
        }
        catch (Exception)
        {
            await ClearStorage(video, cancellationToken);
            throw;
        }
    }

    private async Task ClearStorage(Video video, CancellationToken cancellationToken)
    {
        if (video.Thumb is not null) await storageService.Delete(video.Thumb.Path, cancellationToken);
        if (video.Banner is not null) await storageService.Delete(video.Banner.Path, cancellationToken);
        if (video.ThumbHalf is not null) await storageService.Delete(video.ThumbHalf.Path, cancellationToken);
    }

    private async Task UploadImagesMedia(CreateVideoInput request, Video video, CancellationToken cancellationToken)
    {
        if (request.Thumb is not null)
        {
            var thumbUrl = await storageService.Upload(
                $"{video.Id}-thumb.{request.Thumb.Extension}",
                request.Thumb.FileStream,
                cancellationToken
            );

            video.UpdateThumb(thumbUrl);
        }

        if (request.Banner is not null)
        {
            var bannerUrl = await storageService.Upload(
                $"{video.Id}-banner.{request.Banner.Extension}",
                request.Banner.FileStream,
                cancellationToken
            );

            video.UpdateBanner(bannerUrl);
        }

        if (request.ThumbHalf is not null)
        {
            var thumbHalfUrl = await storageService.Upload(
                $"{video.Id}-thumb-half.{request.ThumbHalf.Extension}",
                request.ThumbHalf.FileStream,
                cancellationToken
            );

            video.UpdateThumbHalf(thumbHalfUrl);
        }
    }

    private async Task ValidateAndAddRelations(CreateVideoInput request, Video video,
        CancellationToken cancellationToken)
    {
        if (request.CategoriesIds?.Count > 0)
        {
            await ValidateCategoryIds(request, cancellationToken);
            request.CategoriesIds!.ToList().ForEach(video.AddCategory);
        }

        if (request.GenresIds?.Count > 0)
        {
            await ValidateGenreIds(request, cancellationToken);
            request.GenresIds!.ToList().ForEach(video.AddGenre);
        }

        if (request.CastMembersIds?.Count > 0)
        {
            await ValidateCastMemberIds(request, cancellationToken);
            request.CastMembersIds!.ToList().ForEach(video.AddCastMember);
        }
    }

    private async Task ValidateCastMemberIds(CreateVideoInput request, CancellationToken cancellationToken)
    {
        var persistenceIds = await castMemberRepository
            .GetIdsListByIds(request.CastMembersIds!.ToList(), cancellationToken);
        if (persistenceIds.Count < request.CastMembersIds!.Count)
        {
            var notFoundIds = request.CastMembersIds.ToList().FindAll(id => !persistenceIds.Contains(id));
            throw new RelatedAggregateException(
                $"Related cast member id (or ids) not found: {string.Join(',', notFoundIds)}."
            );
        }
    }

    private async Task ValidateGenreIds(CreateVideoInput request, CancellationToken cancellationToken)
    {
        var persistenceIds = await genreRepository
            .GetIdsListByIds(request.GenresIds!.ToList(), cancellationToken);
        if (persistenceIds.Count < request.GenresIds!.Count)
        {
            var notFoundIds = request.GenresIds.ToList()
                .FindAll(id => !persistenceIds.Contains(id));
            throw new RelatedAggregateException(
                $"Related genre id (or ids) not found: {string.Join(',', notFoundIds)}.");
        }
    }

    private async Task ValidateCategoryIds(CreateVideoInput request, CancellationToken cancellationToken)
    {
        var persistenceIds = await categoryRepository
            .GetIdsListByIds(request.CategoriesIds!.ToList(), cancellationToken);

        if (persistenceIds.Count < request.CategoriesIds!.Count)
        {
            var notFoundIds = request.CategoriesIds.ToList()
                .FindAll(categoryId => !persistenceIds.Contains(categoryId));

            throw new RelatedAggregateException(
                $"Related category id (or ids) not found: {string.Join(',', notFoundIds)}."
            );
        }
    }
}