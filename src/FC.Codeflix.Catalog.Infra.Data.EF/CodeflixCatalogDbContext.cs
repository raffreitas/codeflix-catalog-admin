using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;

using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF;
public class CodeflixCatalogDbContext(DbContextOptions<CodeflixCatalogDbContext> options)
    : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CastMember> CastMembers => Set<CastMember>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<GenresCategories> GenresCategories => Set<GenresCategories>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new GenreConfiguration());
        modelBuilder.ApplyConfiguration(new GenresCategoriesConfiguration());
    }
}
