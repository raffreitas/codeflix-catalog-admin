using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.UpdateCategory;
public record UpdateCategoryInput(
    Guid Id,
    string Name,
    string? Description = null,
    bool? IsActive = null
) : IRequest<CategoryModelOutput>;