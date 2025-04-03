using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.GetCategory;

public record GetCategoryInput(Guid Id) : IRequest<GetCategoryOutput>;
