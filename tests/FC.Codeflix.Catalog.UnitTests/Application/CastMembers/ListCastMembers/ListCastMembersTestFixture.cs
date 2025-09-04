using FC.Codeflix.Catalog.UnitTests.Application.CastMembers.Common;

using DomainEntity = FC.Codeflix.Catalog.Domain.Entities;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMembers.ListCastMembers;

[CollectionDefinition(nameof(ListCastMembersTestFixture))]
public class ListCastMembersTestFixtureCollection
    : ICollectionFixture<ListCastMembersTestFixture>
{
}

public class ListCastMembersTestFixture
    : CastMemberUseCasesBaseFixture
{
    public List<DomainEntity.CastMember> GetExampleCastMembersList(int quantity)
        => Enumerable
            .Range(1, quantity)
            .Select(_ => GetExampleCastMember())
            .ToList();
}