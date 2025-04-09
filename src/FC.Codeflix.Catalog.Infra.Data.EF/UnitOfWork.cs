
using FC.Codeflix.Catalog.Application.Interfaces;

namespace FC.Codeflix.Catalog.Infra.Data.EF;
public class UnitOfWork(CodeflixCatalogDbContext dbContext) : IUnitOfWork
{
    public async Task Commit(CancellationToken cancellationToken = default)
        => await dbContext.SaveChangesAsync(cancellationToken);

    public Task Rollback(CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
