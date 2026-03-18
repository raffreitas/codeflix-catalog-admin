using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repositories;

namespace FC.Codeflix.Catalog.Application.UseCases.Categories.DeleteCategory;
public class DeleteCategoryUseCase(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : IDeleteCategoryUseCase
{
    public async Task Handle(DeleteCategoryInput request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.Get(request.Id, cancellationToken);

        await categoryRepository.Delete(category, cancellationToken);

        await unitOfWork.Commit(cancellationToken);
    }
}
