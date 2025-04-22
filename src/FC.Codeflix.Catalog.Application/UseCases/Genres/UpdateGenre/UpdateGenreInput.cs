using FC.Codeflix.Catalog.Application.UseCases.Genres.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.UpdateGenre;
public record UpdateGenreInput(Guid Id, string Name, bool? IsActive = null) : IRequest<GenreModelOutput>;
