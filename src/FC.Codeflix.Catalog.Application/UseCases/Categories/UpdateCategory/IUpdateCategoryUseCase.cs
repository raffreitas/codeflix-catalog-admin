using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.UpdateCategory;
public interface IUpdateCategoryUseCase : IRequestHandler<UpdateCategoryInput, CategoryModelOutput>
{
}
