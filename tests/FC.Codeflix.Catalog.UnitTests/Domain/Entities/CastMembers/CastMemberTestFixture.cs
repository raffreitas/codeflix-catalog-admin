using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.UnitTests.Common;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.CastMembers;

[CollectionDefinition(nameof(CastMemberTestFixture))]
public class CastMemberTestFixtureCollection
    : ICollectionFixture<CastMemberTestFixture>
{
}

public class CastMemberTestFixture : BaseFixture
{
    public CastMember GetExampleCastMember()
        => new CastMember(
            GetValidName(),
            GetRandomCastMemberType()
        );

    public string GetValidName()
        => Faker.Name.FullName();

    public CastMemberType GetRandomCastMemberType()
        => (CastMemberType)(new Random()).Next(1, 2);
}