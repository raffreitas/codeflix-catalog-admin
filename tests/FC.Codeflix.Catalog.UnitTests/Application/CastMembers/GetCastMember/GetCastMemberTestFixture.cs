using FC.Codeflix.Catalog.UnitTests.Application.CastMembers.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMembers.GetCastMember;

[CollectionDefinition(nameof(GetCastMemberTestFixture))]
public class GetCastMemberTestFixtureCollection
    : ICollectionFixture<GetCastMemberTestFixture>
{ }

public class GetCastMemberTestFixture
    : CastMemberUseCasesBaseFixture
{ }
