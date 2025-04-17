using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
public class CategoryRepository(CodeflixCatalogDbContext context) : ICategoryRepository
{
    private readonly DbSet<Category> _categories = context.Set<Category>();
    public async Task Insert(Category aggregate, CancellationToken cancellationToken = default)
        => await _categories.AddAsync(aggregate, cancellationToken);

    public async Task<Category> Get(Guid id, CancellationToken cancellationToken = default)
        => await _categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new NotFoundException($"Category '{id}' not found.");

    public Task Update(Category aggregate, CancellationToken cancellationToken = default)
        => Task.FromResult(_categories.Update(aggregate));

    public Task Delete(Category aggregate, CancellationToken cancellationToken = default)
        => Task.FromResult(_categories.Remove(aggregate));

    public async Task<SearchOutput<Category>> Search(SearchInput input, CancellationToken cancellationToken = default)
    {
        var toSkip = (input.Page - 1) * input.PerPage;
        var query = _categories.AsNoTracking();

        query = AddOrderToQuery(query, input.OrderBy, input.Order);

        if (!string.IsNullOrWhiteSpace(input.Search))
            query = query.Where(x => x.Name.Contains(input.Search));

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(toSkip)
            .Take(input.PerPage)
            .ToListAsync(cancellationToken);

        return new SearchOutput<Category>(
            currentPage: input.Page,
            perPage: input.PerPage,
            total: total,
            items: items
        );
    }

    private static IQueryable<Category> AddOrderToQuery(IQueryable<Category> query, string orderProperty, SearchOrder order)
    {
        var orderedQuery = (orderProperty.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name),
            ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name),
            ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.Name)
        };

        return orderedQuery.ThenBy(x => x.CreatedAt);
    }

    public async Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _categories
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
    }
}
