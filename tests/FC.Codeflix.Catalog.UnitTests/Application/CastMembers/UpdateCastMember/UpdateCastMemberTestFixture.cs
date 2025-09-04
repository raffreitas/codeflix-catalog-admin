using FC.Codeflix.Catalog.UnitTests.Application.CastMembers.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMembers.UpdateCastMember;

[CollectionDefinition(nameof(UpdateCastMemberTestFixture))]
public class UpdateCastMemberTestFixtureCollection
    : ICollectionFixture<UpdateCastMemberTestFixture>
{ }

public class UpdateCastMemberTestFixture 
    : CastMemberUseCasesBaseFixture
{ }
