using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;

using Microsoft.EntityFrameworkCore;

using DomainEntity = FC.Codeflix.Catalog.Domain.Entities;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genres.Common;

public class GenrePersistence(CodeflixCatalogDbContext context)
{
    public async Task InsertList(List<DomainEntity.Genre> genres)
    {
        await context.AddRangeAsync(genres);
        await context.SaveChangesAsync();
    }

    public async Task InsertGenresCategoriesRelationsList(List<GenresCategories> relations)
    {
        await context.AddRangeAsync(relations);
        await context.SaveChangesAsync();
    }

    public async Task<DomainEntity.Genre?> GetById(Guid id)
        => await context.Genres.AsNoTracking()
            .FirstOrDefaultAsync(genre => genre.Id == id);

    internal async Task<List<GenresCategories>> GetGenresCategoriesRelationsByGenreId(Guid id)
        => await context.GenresCategories.AsNoTracking()
            .Where(relation => relation.GenreId == id)
            .ToListAsync();
}