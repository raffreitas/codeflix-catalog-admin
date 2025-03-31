namespace FC.Codeflix.Catalog.Application.UseCases.Categories.CreateCategory;
public record CreateCategoryInput
{
    public string Name { get; }
    public string Description { get; }
    public bool IsActive { get; }
    public CreateCategoryInput(
        string name, 
        string? description = null,
        bool isActive = true)
    {
        Name = name;
        Description = description ?? string.Empty;
        IsActive = isActive;
    }
};
