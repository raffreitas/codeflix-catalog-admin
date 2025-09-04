using FC.Codeflix.Catalog.UnitTests.Application.CastMembers.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMembers.DeleteCastMember;

[CollectionDefinition(nameof(DeleteCastMemberFixture))]
public class DeleteCastMemberFixtureCollection
    : ICollectionFixture<DeleteCastMemberFixture>
{ }

public class DeleteCastMemberFixture
    : CastMemberUseCasesBaseFixture
{ }
