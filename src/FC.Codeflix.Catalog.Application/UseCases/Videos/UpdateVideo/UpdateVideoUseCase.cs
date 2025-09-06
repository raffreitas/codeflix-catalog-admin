using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Videos.Common;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.Domain.Validations;

namespace FC.Codeflix.Catalog.Application.UseCases.Videos.UpdateVideo;

public sealed class UpdateVideoUseCase(
    IVideoRepository videoRepository,
    IGenreRepository genreRepository,
    ICategoryRepository categoryRepository,
    ICastMemberRepository castMemberRepository,
    IUnitOfWork unitOfWork,
    IStorageService storageService
) : IUpdateVideoUseCase
{
    public async Task<VideoModelOutput> Handle(UpdateVideoInput request, CancellationToken cancellationToken)
    {
        var video = await videoRepository.Get(request.VideoId, cancellationToken);
        video.Update(
            request.Title,
            request.Description,
            request.YearLaunched,
            request.Duration,
            request.Opened,
            request.Published,
            request.Rating
        );

        await ValidateAndAddRelations(request, video, cancellationToken);
        await UploadImagesMedia(video, request, cancellationToken);

        var validationHandler = new NotificationValidationHandler();
        video.Validate(validationHandler);
        if (validationHandler.HasErrors())
            throw new EntityValidationException("There are validation errors", validationHandler.Errors);

        await videoRepository.Update(video, cancellationToken);
        await unitOfWork.Commit(cancellationToken);
        return VideoModelOutput.FromVideo(video);
    }

    private async Task ValidateAndAddRelations(UpdateVideoInput request, Video video,
        CancellationToken cancellationToken)
    {
        if (request.GenresIds is not null)
        {
            video.RemoveAllGenres();
            if (request.GenresIds.Count > 0)
            {
                await ValidateGenresIds(request, cancellationToken);
                request.GenresIds!.ToList().ForEach(video.AddGenre);
            }
        }

        if (request.CategoriesIds is not null)
        {
            video.RemoveAllCategories();
            if (request.CategoriesIds.Count > 0)
            {
                await ValidateCategoriesIds(request, cancellationToken);
                request.CategoriesIds!.ToList().ForEach(video.AddCategory);
            }
        }

        if (request.CastMembersIds is not null)
        {
            video.RemoveAllCastMembers();
            if (request.CastMembersIds.Count > 0)
            {
                await ValidateCastMembersIds(request, cancellationToken);
                request.CastMembersIds!.ToList().ForEach(video.AddCastMember);
            }
        }
    }

    private async Task ValidateGenresIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds = await genreRepository
            .GetIdsListByIds(input.GenresIds!.ToList(), cancellationToken);
        if (persistenceIds.Count < input.GenresIds!.Count)
        {
            var notFoundIds = input.GenresIds!.ToList()
                .FindAll(id => !persistenceIds.Contains(id));
            throw new RelatedAggregateException(
                $"Related genre id (or ids) not found: {string.Join(',', notFoundIds)}.");
        }
    }

    private async Task ValidateCategoriesIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds = await categoryRepository
            .GetIdsListByIds(input.CategoriesIds!.ToList(), cancellationToken);
        if (persistenceIds.Count < input.CategoriesIds!.Count)
        {
            var notFoundIds = input.CategoriesIds!.ToList()
                .FindAll(id => !persistenceIds.Contains(id));
            throw new RelatedAggregateException(
                $"Related category id (or ids) not found: {string.Join(',', notFoundIds)}.");
        }
    }

    private async Task ValidateCastMembersIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds = await castMemberRepository
            .GetIdsListByIds(input.CastMembersIds!.ToList(), cancellationToken);
        if (persistenceIds.Count < input.CastMembersIds!.Count)
        {
            var notFoundIds = input.CastMembersIds!.ToList()
                .FindAll(id => !persistenceIds.Contains(id));
            throw new RelatedAggregateException(
                $"Related cast member(s) id (or ids) not found: {string.Join(',', notFoundIds)}.");
        }
    }

    private async Task UploadImagesMedia(
        Video video,
        UpdateVideoInput input,
        CancellationToken cancellationToken)
    {
        if (input.Banner is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Banner), input.Banner.Extension);
            var bannerUrl = await storageService.Upload(
                fileName,
                input.Banner.FileStream,
                input.Banner.ContentType,
                cancellationToken);
            video.UpdateBanner(bannerUrl);
        }

        if (input.Thumb is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Thumb), input.Thumb.Extension);
            var thumbUrl = await storageService.Upload(
                fileName,
                input.Thumb.FileStream,
                input.Thumb.ContentType,
                cancellationToken);
            video.UpdateThumb(thumbUrl);
        }

        if (input.ThumbHalf is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.ThumbHalf), input.ThumbHalf.Extension);
            var thumbUrl = await storageService.Upload(
                fileName,
                input.ThumbHalf.FileStream,
                input.ThumbHalf.ContentType,
                cancellationToken);
            video.UpdateThumbHalf(thumbUrl);
        }
    }
}