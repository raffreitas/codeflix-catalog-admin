using Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMembers.Common;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMembers.CreateCastMember;

[CollectionDefinition(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTestFixtureCollection
    : ICollectionFixture<CreateCastMemberTestFixture>
{
}

public class CreateCastMemberTestFixture
    : CastMemberUseCasesBaseFixture
{
}
