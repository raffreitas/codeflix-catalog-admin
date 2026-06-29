using Codeflix.Catalog.Application.UseCases.Genres.Common;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Genres.CreateGenre;

internal interface ICreateGenreUseCase : IRequestHandler<CreateGenreInput, GenreModelOutput>
{
}
