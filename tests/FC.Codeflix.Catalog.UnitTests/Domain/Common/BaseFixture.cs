using Bogus;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Common;
public abstract class BaseFixture
{
    public Faker Faker { get; set; }

    protected BaseFixture()
        => Faker = new Faker(locale: "pt_BR");
}
