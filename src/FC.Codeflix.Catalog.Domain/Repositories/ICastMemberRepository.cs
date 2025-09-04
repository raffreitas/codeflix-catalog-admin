using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

namespace FC.Codeflix.Catalog.Domain.Repositories;

public interface ICastMemberRepository : IGenericRepository<CastMember>, ISearchableRepository<CastMember>
{
    public Task<IReadOnlyList<Guid>> GetIdsListByIds(
        List<Guid> ids,
        CancellationToken cancellationToken
    );
}