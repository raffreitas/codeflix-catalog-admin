using Codeflix.Catalog.EndToEndTests.Base;

namespace Codeflix.Catalog.EndToEndTests.Api.Authorization;

[CollectionDefinition(nameof(AuthorizationTestFixture))]
public class AuthorizationTestFixtureCollection : ICollectionFixture<AuthorizationTestFixture>
{
}

public sealed class AuthorizationTestFixture : BaseFixture
{
}