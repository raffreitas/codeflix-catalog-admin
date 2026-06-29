using Codeflix.Catalog.Application.UseCases.Categories.Common;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Categories.CreateCategory;
public interface ICreateCategoryUseCase : IRequestHandler<CreateCategoryInput, CategoryModelOutput>
{
}