using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.ListCategories;
public interface IListCategoriesUseCase : IRequestHandler<ListCategoriesInput, ListCategoriesOutput>
{
}
