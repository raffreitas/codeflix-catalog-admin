using FC.Codeflix.Catalog.Domain.Entities;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.GetCategory;

public record GetCategoryOutput
{
    public Guid Id { get; set; }
    public string Name { get; }
    public string Description { get; }
    public bool IsActive { get; }
    public DateTime CreatedAt { get; set; }

    public GetCategoryOutput(
        Guid id,
        string name,
        string description,
        bool isActive,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = createdAt;
    }

    public static GetCategoryOutput FromCategory(Category category)
        => new(
            id: category.Id,
            name: category.Name,
            description: category.Description,
            isActive: category.IsActive,
            createdAt: category.CreatedAt);
}