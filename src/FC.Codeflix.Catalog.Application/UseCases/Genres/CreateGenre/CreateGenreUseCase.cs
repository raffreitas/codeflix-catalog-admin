using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Genres.Common;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Repositories;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.CreateGenre;
public class CreateGenreUseCase(
    IGenreRepository genreRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork)
    : ICreateGenreUseCase
{
    public async Task<GenreModelOutput> Handle(CreateGenreInput request, CancellationToken cancellationToken)
    {
        var genre = new Genre(request.Name, request.IsActive);

        if (request.CategoriesIds is { Count: > 0 })
        {
            await ValidateCategoryIds(request.CategoriesIds, cancellationToken);
            request.CategoriesIds.ForEach(genre.AddCategory);
        }

        await genreRepository.Insert(genre, cancellationToken);
        await unitOfWork.Commit(cancellationToken);

        return GenreModelOutput.FromGenre(genre);
    }

    private async Task ValidateCategoryIds(List<Guid> categoryIds, CancellationToken cancellationToken)
    {
        var idsInPersistence = await categoryRepository.GetIdsListByIds(categoryIds, cancellationToken);
        if (idsInPersistence.Count < categoryIds.Count)
        {
            var notFoundIds = categoryIds
                .Where(id => !idsInPersistence.Contains(id));
            throw new RelatedAggregateException(
                $"Related category id (or ids) not found: {string.Join(", ", notFoundIds)}");
        }
    }
}
