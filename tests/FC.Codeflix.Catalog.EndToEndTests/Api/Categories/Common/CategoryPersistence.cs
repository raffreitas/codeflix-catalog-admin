using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Infra.Data.EF;

using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Categories.Common;
public class CategoryPersistence(CodeflixCatalogDbContext dbContext)
{
    public async Task<Category?> GetById(Guid id) 
        => await dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
}
