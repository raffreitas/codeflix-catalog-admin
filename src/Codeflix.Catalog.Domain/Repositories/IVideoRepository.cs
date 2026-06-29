using Codeflix.Catalog.Domain.Entities;
using Codeflix.Catalog.Domain.SeedWork;
using Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

namespace Codeflix.Catalog.Domain.Repositories;

public interface IVideoRepository : IGenericRepository<Video>, ISearchableRepository<Video>
{
}