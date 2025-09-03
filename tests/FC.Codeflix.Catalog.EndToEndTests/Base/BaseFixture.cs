using Bogus;

using FC.Codeflix.Catalog.Infra.Data.EF;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;

public abstract class BaseFixture
{
    private readonly string _dbConnectionString;
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
        var configuration = WebAppFactory.Services.GetRequiredService(typeof(IConfiguration));
        ArgumentNullException.ThrowIfNull(configuration);
        _dbConnectionString = ((IConfiguration)configuration).GetConnectionString("CatalogDb")!;
    }

    public CodeflixCatalogDbContext CreateDbContext()
    {
        var dbContext = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                .UseMySql(_dbConnectionString, ServerVersion.AutoDetect(_dbConnectionString))
                .Options
        );
        return dbContext;
    }

    public void CleanPersistence()
    {
        using var dbContext = CreateDbContext();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }
}