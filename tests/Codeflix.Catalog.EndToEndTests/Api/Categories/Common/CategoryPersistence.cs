using Codeflix.Catalog.Domain.Entities;
using Codeflix.Catalog.Infra.Data.EF;

using Microsoft.EntityFrameworkCore;

namespace Codeflix.Catalog.EndToEndTests.Api.Categories.Common;
public class CategoryPersistence(CodeflixCatalogDbContext dbContext)
{
    public async Task<Category?> GetById(Guid id)
        => await dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task InsertList(List<Category> categories)
    {
        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync();
    }
}
