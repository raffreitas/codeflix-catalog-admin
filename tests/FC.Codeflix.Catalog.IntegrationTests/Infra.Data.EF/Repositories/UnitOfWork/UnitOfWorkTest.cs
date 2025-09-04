using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using UoW = FC.Codeflix.Catalog.Infra.Data.EF;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.UnitOfWork;

[Collection(nameof(UnitOfWorkTestFixture))]
public class UnitOfWorkTest(UnitOfWorkTestFixture fixture)
{
    [Fact(DisplayName = nameof(Commit))]
    [Trait("Integration/Infra.Data.EF", "UnitOfWork - Persistence")]
    public async Task Commit()
    {
        var dbContext = fixture.CreateDbContext();
        var categoriesList = fixture.GetExampleCategoryList();
        await dbContext.AddRangeAsync(categoriesList);
        var unitOfWork = new UoW.UnitOfWork(dbContext);

        await unitOfWork.Commit();

        var categories = await fixture
            .CreateDbContext(preserveData: true)
            .Categories
            .AsNoTracking()
            .ToListAsync();

        categories.Should().HaveCount(categoriesList.Count);
    }

    [Fact(DisplayName = nameof(Rollback))]
    [Trait("Integration/Infra.Data.EF", "UnitOfWork - Persistence")]
    public async Task Rollback()
    {
        var dbContext = fixture.CreateDbContext();
        var categoriesList = fixture.GetExampleCategoryList();
        await dbContext.AddRangeAsync(categoriesList);
        var unitOfWork = new UoW.UnitOfWork(dbContext);

        await unitOfWork.Rollback();

        var categories = await fixture
            .CreateDbContext(preserveData: true)
            .Categories
            .AsNoTracking()
            .ToListAsync();

        categories.Should().BeEmpty();
    }
}
