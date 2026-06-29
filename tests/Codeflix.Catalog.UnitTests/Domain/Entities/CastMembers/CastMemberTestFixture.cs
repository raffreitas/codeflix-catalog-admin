using Codeflix.Catalog.UnitTests.Common;

using Codeflix.Catalog.Domain.Entities;
using Codeflix.Catalog.Domain.Enum;

namespace Codeflix.Catalog.UnitTests.Domain.Entities.CastMembers;

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