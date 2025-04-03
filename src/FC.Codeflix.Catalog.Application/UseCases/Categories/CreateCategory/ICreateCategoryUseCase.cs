using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.CreateCategory;
public interface ICreateCategoryUseCase 
    : IRequestHandler<CreateCategoryInput, CreateCategoryOutput>
{
    public Task<CreateCategoryOutput> Handle(CreateCategoryInput input, CancellationToken cancellationToken = default);
}
