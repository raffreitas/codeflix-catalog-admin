namespace FC.Codeflix.Catalog.Api.Models.Categories;

public record UpdateCategoryApiInput
{
    public string Name { get; set; }
    public string? Description { get; set; } = null;
    public bool? IsActive { get; set; } = null;

    public UpdateCategoryApiInput(
        string name, 
        string? description = null, 
        bool? isActive = null)
    {
        Name = name;
        Description = description;
        IsActive = isActive;
    }
}
