using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.DeleteCategory;
public record DeleteCategoryInput(Guid Id) : IRequest;
