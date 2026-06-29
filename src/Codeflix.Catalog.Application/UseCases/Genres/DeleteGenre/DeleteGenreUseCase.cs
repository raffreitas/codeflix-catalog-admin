using Codeflix.Catalog.Application.Interfaces;

using Codeflix.Catalog.Domain.Repositories;

namespace Codeflix.Catalog.Application.UseCases.Genres.DeleteGenre;

public class DeleteGenreUseCase(IGenreRepository genreRepository, IUnitOfWork unitOfWork) : IDeleteGenreUseCase
{
    public async Task Handle(DeleteGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await genreRepository.Get(request.Id, cancellationToken);
        await genreRepository.Delete(genre, cancellationToken);
        await unitOfWork.Commit(cancellationToken);
    }
}
