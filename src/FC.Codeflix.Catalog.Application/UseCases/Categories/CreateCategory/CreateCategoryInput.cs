using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.CreateCategory;
public record CreateCategoryInput : IRequest<CategoryModelOutput>
{
    public string Name { get; set; }
    public string Description { get; set; }
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