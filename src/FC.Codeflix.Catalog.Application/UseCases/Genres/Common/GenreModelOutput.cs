using FC.Codeflix.Catalog.Domain.Entities;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.Common;

public record GenreModelOutput(
    Guid Id,
    string Name,
    bool IsActive,
    DateTime CreatedAt,
    IReadOnlyList<GenreModelOutputCategory> Categories)
{
    public Guid Id { get; set; } = Id;
    public string Name { get; set; } = Name;
    public bool IsActive { get; set; } = IsActive;
    public DateTime CreatedAt { get; set; } = CreatedAt;
    public IReadOnlyList<GenreModelOutputCategory> Categories { get; set; } = Categories;

    public static GenreModelOutput FromGenre(Genre genre)
        => new(
            Id: genre.Id,
            Name: genre.Name,
            IsActive: genre.IsActive,
            CreatedAt: genre.CreatedAt,
            Categories: genre.Categories.Select(categoryId => new GenreModelOutputCategory(categoryId)
            ).ToList().AsReadOnly()
        );
}

public class GenreModelOutputCategory(Guid id, string? name = null)
{
    public Guid Id { get; set; } = id;
    public string? Name { get; set; } = name;
}