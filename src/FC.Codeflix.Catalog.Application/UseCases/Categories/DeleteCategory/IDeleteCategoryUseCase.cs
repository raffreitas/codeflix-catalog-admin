using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.DeleteCategory;
internal interface IDeleteCategoryUseCase : IRequestHandler<DeleteCategoryInput>
{
}
