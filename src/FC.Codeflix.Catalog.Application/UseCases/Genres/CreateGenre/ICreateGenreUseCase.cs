using FC.Codeflix.Catalog.Application.UseCases.Genres.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.CreateGenre;

internal interface ICreateGenreUseCase : IRequestHandler<CreateGenreInput, GenreModelOutput>
{
}
