using FC.Codeflix.Catalog.Application.UseCases.Genres.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.CreateGenre;

public record CreateGenreInput(string Name, bool IsActive, List<Guid>? CategoriesIds = null)
    : IRequest<GenreModelOutput>
{
    public List<Guid>? CategoriesIds { get; set; } = CategoriesIds;
}