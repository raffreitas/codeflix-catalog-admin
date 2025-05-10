using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.DeleteGenre;

public interface IDeleteGenreUseCase : IRequestHandler<DeleteGenreInput> { }