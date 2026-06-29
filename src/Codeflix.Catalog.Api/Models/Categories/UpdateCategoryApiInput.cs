namespace Codeflix.Catalog.Api.Models.Categories;

public record UpdateCategoryApiInput(
    string Name,
    string? Description = null,
    bool? IsActive = null)
{
    public string Name { get; set; } = Name;
    public string? Description { get; set; } = Description;
    public bool? IsActive { get; set; } = IsActive;
}
