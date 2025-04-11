using Bogus;

using FC.Codeflix.Catalog.Infra.Data.EF;

using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;
public abstract class BaseFixture
{
    protected Faker Faker { get; }
    public CustomWebApplicationFactory<Program> WebAppFactory { get; }
    public HttpClient HttpClient { get; }
    public ApiClient ApiClient { get; }
    protected BaseFixture()
    {
        Faker = new Faker("pt_BR");
        WebAppFactory = new CustomWebApplicationFactory<Program>();
        HttpClient = WebAppFactory.CreateClient();
        ApiClient = new ApiClient(HttpClient);
    }

    public CodeflixCatalogDbContext CreateDbContext(bool preserveData = false)
    {
        var dbContext = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                .UseInMemoryDatabase("e2e-tests-db")
                .Options
        );
        return dbContext;
    }
}
