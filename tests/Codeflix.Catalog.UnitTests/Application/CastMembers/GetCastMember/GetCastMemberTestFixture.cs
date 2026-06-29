using Codeflix.Catalog.UnitTests.Application.CastMembers.Common;

namespace Codeflix.Catalog.UnitTests.Application.CastMembers.GetCastMember;

[CollectionDefinition(nameof(GetCastMemberTestFixture))]
public class GetCastMemberTestFixtureCollection
    : ICollectionFixture<GetCastMemberTestFixture>
{ }

public class GetCastMemberTestFixture
    : CastMemberUseCasesBaseFixture
{ }
