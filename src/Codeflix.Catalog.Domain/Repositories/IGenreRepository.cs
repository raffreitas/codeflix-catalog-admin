using Codeflix.Catalog.Domain.Entities;
using Codeflix.Catalog.Domain.SeedWork;
using Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

namespace Codeflix.Catalog.Domain.Repositories;

public interface IGenreRepository : IGenericRepository<Genre>, ISearchableRepository<Genre>
{
    public Task<IReadOnlyList<Guid>> GetIdsListByIds(
        List<Guid> ids,
        CancellationToken cancellationToken
    );

    public Task<IReadOnlyList<Genre>> GetListByIds(
        List<Guid> ids,
        CancellationToken cancellationToken
    );
}