using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Genres.Common;
using FC.Codeflix.Catalog.Domain.Repositories;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.UpdateGenre;
public class UpdateGenreUseCase(
    IGenreRepository genreRepository,
    IUnitOfWork unitOfWork,
    ICategoryRepository categoryRepository)
    : IUpdateGenreUseCase
{
    public async Task<GenreModelOutput> Handle(UpdateGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await genreRepository.Get(request.Id, cancellationToken);

        genre.Update(request.Name);

        if (request.IsActive is not null && request.IsActive != genre.IsActive)
        {
            if (request.IsActive.Value) genre.Activate();
            else genre.Deactivate();
        }

        if (request.CategoriesIds is not null)
        {
            genre.RemoveAllCategories();
            if (request.CategoriesIds.Count > 0)
            {
                await ValidateCategoryIds(request.CategoriesIds, cancellationToken);
                request.CategoriesIds.ForEach(genre.AddCategory);
            }
        }

        await genreRepository.Update(genre, cancellationToken);
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
