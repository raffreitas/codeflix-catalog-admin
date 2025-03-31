namespace FC.Codeflix.Catalog.Application.UseCases.Categories.CreateCategory;
public interface ICreateCategoryUseCase
{
    public Task<CreateCategoryOutput> Handle(CreateCategoryInput input, CancellationToken cancellationToken = default);
}
