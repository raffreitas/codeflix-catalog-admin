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
        => await _categories.FindAsync([id], cancellationToken) ?? throw new NotFoundException($"Category '{id}' not found.");

    public Task Update(Category aggregate, CancellationToken cancellationToken = default)
        => Task.FromResult(_categories.Update(aggregate));

    public Task Delete(Category aggregate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SearchOutput<Category>> Search(SearchInput input, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
