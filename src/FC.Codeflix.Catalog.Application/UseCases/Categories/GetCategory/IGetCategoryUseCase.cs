using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.GetCategory;
public interface IGetCategoryUseCase : IRequestHandler<GetCategoryInput, GetCategoryOutput>
{
}
