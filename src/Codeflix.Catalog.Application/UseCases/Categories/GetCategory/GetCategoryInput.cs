using Codeflix.Catalog.Application.UseCases.Categories.Common;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Categories.GetCategory;

public record GetCategoryInput(Guid Id) : IRequest<CategoryModelOutput>;