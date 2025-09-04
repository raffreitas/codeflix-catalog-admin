using FC.Codeflix.Catalog.UnitTests.Application.CastMembers.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMembers.CreateCastMember;

[CollectionDefinition(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTestFixtureCollection
    : ICollectionFixture<CreateCastMemberTestFixture>
{ }

public class CreateCastMemberTestFixture
    : CastMemberUseCasesBaseFixture
{
}
