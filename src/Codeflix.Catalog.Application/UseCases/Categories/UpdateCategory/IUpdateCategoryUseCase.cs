using Codeflix.Catalog.Application.UseCases.Categories.Common;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Categories.UpdateCategory;
public interface IUpdateCategoryUseCase : IRequestHandler<UpdateCategoryInput, CategoryModelOutput>
{
}
