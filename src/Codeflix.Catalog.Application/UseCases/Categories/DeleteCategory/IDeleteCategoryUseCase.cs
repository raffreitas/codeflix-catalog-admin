using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Categories.DeleteCategory;
internal interface IDeleteCategoryUseCase : IRequestHandler<DeleteCategoryInput>
{
}
