namespace FC.Codeflix.Catalog.Api.Models.Genres;

public sealed record UpdateGenreApiInput(string Name, bool? IsActive = null, List<Guid>? CategoriesId = null);