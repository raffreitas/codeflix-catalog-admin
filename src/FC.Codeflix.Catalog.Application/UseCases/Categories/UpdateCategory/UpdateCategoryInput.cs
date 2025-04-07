using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.UpdateCategory;
public record UpdateCategoryInput(
    Guid Id, 
    string Name, 
    string Description, 
    bool IsActive
) : IRequest<CategoryModelOutput>;