using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.UpdateCategory;
public record UpdateCategoryInput(Guid Id, string Name, string? Description = null, bool? IsActive = null)
    : IRequest<CategoryModelOutput>
{
    public Guid Id { get; set; } = Id;
    public string Name { get; set; } = Name;
    public string? Description { get; set; } = Description;
    public bool? IsActive { get; set; } = IsActive;
}