using Codeflix.Catalog.Application.UseCases.Categories.Common;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Categories.GetCategory;
public interface IGetCategoryUseCase : IRequestHandler<GetCategoryInput, CategoryModelOutput>
{
}