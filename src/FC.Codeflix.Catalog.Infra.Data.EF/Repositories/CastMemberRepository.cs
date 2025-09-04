using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

public class CastMemberRepository(CodeflixCatalogDbContext context) : ICastMemberRepository
{
    private DbSet<CastMember> CastMembers => context.Set<CastMember>();

    public async Task Insert(CastMember aggregate, CancellationToken cancellationToken)
        => await CastMembers.AddAsync(aggregate, cancellationToken);

    public Task Delete(CastMember aggregate, CancellationToken _)
        => Task.FromResult(CastMembers.Remove(aggregate));

    public async Task<CastMember> Get(Guid id, CancellationToken cancellationToken)
    {
        var castmember = await CastMembers.AsNoTracking().FirstOrDefaultAsync(
            x => x.Id == id,
            cancellationToken
        );
        NotFoundException.ThrowIfNull(castmember, $"CastMember '{id}' not found.");
        return castmember!;
    }

    public async Task<SearchOutput<CastMember>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;
        var query = CastMembers.AsNoTracking();
        query = AddOrderToQuery(query, input.OrderBy, input.Order);
        if (!String.IsNullOrWhiteSpace(input.Search))
            query = query.Where(x => x.Name.Contains(input.Search));
        var items = await query
            .Skip(toSkip)
            .Take(input.PerPage)
            .ToListAsync(cancellationToken: cancellationToken);
        var count = await query.CountAsync(cancellationToken: cancellationToken);
        return new SearchOutput<CastMember>(
            input.Page,
            input.PerPage,
            count,
            items.AsReadOnly()
        );
    }

    public Task Update(CastMember aggregate, CancellationToken _)
        => Task.FromResult(CastMembers.Update(aggregate));

    private IQueryable<CastMember> AddOrderToQuery(
        IQueryable<CastMember> query,
        string orderProperty,
        SearchOrder order
    )
    {
        var orderedQuery = (orderProperty.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name)
                .ThenBy(x => x.Id),
            ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name)
                .ThenByDescending(x => x.Id),
            ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
        };
        return orderedQuery;
    }

    public async Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> ids, CancellationToken cancellationToken)
        => await CastMembers
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
}