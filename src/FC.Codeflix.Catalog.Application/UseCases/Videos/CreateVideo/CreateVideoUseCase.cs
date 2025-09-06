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
    IUnitOfWork uow
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

        if (request.CategoriesIds?.Count > 0)
        {
            var persistenceIds = await categoryRepository.GetIdsListByIds([..request.CategoriesIds], cancellationToken);

            if (persistenceIds.Count < request.CategoriesIds.Count)
            {
                var notFoundIds = request.CategoriesIds.ToList()
                    .FindAll(categoryId => !persistenceIds.Contains(categoryId));

                throw new RelatedAggregateException(
                    $"Related category id (or ids) not found: {string.Join(',', notFoundIds)}."
                );
            }

            request.CategoriesIds!.ToList().ForEach(video.AddCategory);
        }

        if ((request.GenresIds?.Count ?? 0) > 0)
        {
            var persistenceIds = await genreRepository.GetIdsListByIds(
                request.GenresIds!.ToList(), cancellationToken);
            if (persistenceIds.Count < request.GenresIds!.Count)
            {
                var notFoundIds = request.GenresIds!.ToList()
                    .FindAll(id => !persistenceIds.Contains(id));
                throw new RelatedAggregateException(
                    $"Related genre id (or ids) not found: {string.Join(',', notFoundIds)}.");
            }

            request.GenresIds!.ToList().ForEach(video.AddGenre);
        }

        if ((request.CastMembersIds?.Count ?? 0) > 0)
        {
            var persistenceIds = await castMemberRepository.GetIdsListByIds(
                request.CastMembersIds!.ToList(), cancellationToken);
            if (persistenceIds.Count < request.CastMembersIds!.Count)
            {
                var notFoundIds = request.CastMembersIds!.ToList().FindAll(id => !persistenceIds.Contains(id));
                throw new RelatedAggregateException(
                    $"Related cast member id (or ids) not found: {string.Join(',', notFoundIds)}."
                );
            }

            request.CastMembersIds!.ToList().ForEach(video.AddCastMember);
        }

        await videoRepository.Insert(video, cancellationToken);
        await uow.Commit(cancellationToken);

        return CreateVideoOutput.FromVideo(video);
    }
}