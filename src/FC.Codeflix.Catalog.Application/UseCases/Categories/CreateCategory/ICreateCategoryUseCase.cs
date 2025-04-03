using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.CreateCategory;
public interface ICreateCategoryUseCase : IRequestHandler<CreateCategoryInput, CategoryModelOutput>
{
}
