using Bogus;

namespace FC.Codeflix.Catalog.UnitTests.Common;

public abstract class BaseFixture
{
    public Faker Faker { get; set; } = new(locale: "pt_BR");

    public bool GetRandomBoolean() => Faker.Random.Bool();
}