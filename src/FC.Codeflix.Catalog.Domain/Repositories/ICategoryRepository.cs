using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

namespace FC.Codeflix.Catalog.Domain.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>, ISearchableRepository<Category>
{
    public Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> ids, CancellationToken cancellationToken = default);
    public Task<IReadOnlyList<Category>> GetListByIds(List<Guid> ids, CancellationToken cancellationToken = default);
}