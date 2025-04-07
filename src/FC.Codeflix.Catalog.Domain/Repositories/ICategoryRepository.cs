using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

namespace FC.Codeflix.Catalog.Domain.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>, ISearchableRepository<Category>
{
}