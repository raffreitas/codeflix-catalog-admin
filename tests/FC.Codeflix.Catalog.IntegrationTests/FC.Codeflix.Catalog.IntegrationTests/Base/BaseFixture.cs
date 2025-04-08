using Bogus;

namespace FC.Codeflix.Catalog.IntegrationTests.Base;
public abstract class BaseFixture
{
    protected Faker Faker { get; } = new Faker("pt_BR");
}
