using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Genres.DeleteGenre;

public interface IDeleteGenreUseCase : IRequestHandler<DeleteGenreInput> { }