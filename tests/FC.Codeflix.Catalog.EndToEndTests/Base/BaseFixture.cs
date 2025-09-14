using Bogus;

using FC.Codeflix.Catalog.Infra.Data.EF;

using Keycloak.AuthServices.Authentication;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;

public abstract class BaseFixture : IDisposable
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
        var configuration = WebAppFactory.Services.GetRequiredService<IConfiguration>();
        var keycloakOptions = configuration.GetSection(KeycloakAuthenticationOptions.Section)
            .Get<KeycloakAuthenticationOptions>();
        ApiClient = new ApiClient(HttpClient, keycloakOptions!, configuration);
        _dbConnectionString = configuration.GetConnectionString("CatalogDb")!;
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

    public void Dispose()
    {
        WebAppFactory.Dispose();
    }
}