using FC.Codeflix.Catalog.Domain.Entities;

namespace FC.Codeflix.Catalog.Application.UseCases.Genres.Common;

public record GenreModelOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public IReadOnlyList<GenreModelOutputCategory> Categories { get; set; }

    public GenreModelOutput(Guid id, string name, bool isActive, DateTime createdAt,
        IReadOnlyList<GenreModelOutputCategory> categories)
    {
        Id = id;
        Name = name;
        IsActive = isActive;
        CreatedAt = createdAt;
        Categories = categories;
    }

    public static GenreModelOutput FromGenre(Genre genre)
        => new(
            id: genre.Id,
            name: genre.Name,
            isActive: genre.IsActive,
            createdAt: genre.CreatedAt,
            categories: genre.Categories.Select(categoryId => new GenreModelOutputCategory(categoryId)
            ).ToList().AsReadOnly()
        );
}

public class GenreModelOutputCategory(Guid id, string? name = null)
{
    public Guid Id { get; set; } = id;
    public string? Name { get; set; } = name;
}