using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Categories.DeleteCategory;
public record DeleteCategoryInput(Guid Id) : IRequest;
