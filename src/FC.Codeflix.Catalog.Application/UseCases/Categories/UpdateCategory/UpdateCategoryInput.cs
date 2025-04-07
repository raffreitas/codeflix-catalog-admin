using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.UpdateCategory;
public record UpdateCategoryInput : IRequest<CategoryModelOutput>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; } = null;
    public bool? IsActive { get; set; } = null;
    public UpdateCategoryInput(Guid id, string name, string? description = null, bool? isActive = null)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = isActive;
    }
};