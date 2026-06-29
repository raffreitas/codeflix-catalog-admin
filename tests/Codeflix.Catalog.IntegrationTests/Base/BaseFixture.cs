using Bogus;

using Codeflix.Catalog.Infra.Data.EF;

using Microsoft.EntityFrameworkCore;

namespace Codeflix.Catalog.IntegrationTests.Base;
public abstract class BaseFixture
{
    protected Faker Faker { get; } = new Faker("pt_BR");
    public CodeflixCatalogDbContext CreateDbContext(bool preserveData = false)
    {
        var dbContext = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                .UseInMemoryDatabase("integration-tests-db")
                .Options
        );

        if (!preserveData)
            dbContext.Database.EnsureDeleted();

        return dbContext;
    }
}
