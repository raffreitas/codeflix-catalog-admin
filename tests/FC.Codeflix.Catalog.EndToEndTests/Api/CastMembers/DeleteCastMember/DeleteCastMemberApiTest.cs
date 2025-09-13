using System.Net;

using FC.Codeflix.Catalog.EndToEndTests.Api.CastMembers.Common;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMembers.DeleteCastMember;

[Collection(nameof(CastMemberApiBaseFixture))]
public class DeleteCastMemberApiTest(CastMemberApiBaseFixture fixture) : IDisposable
{
    [Fact(DisplayName = nameof(Delete))]
    [Trait("EndToEnd/API", "CatMembers/Delete - EndPoints")]
    public async Task Delete()
    {
        var examples = fixture.GetExampleCastMembersList(5);
        var example = examples[2];
        await fixture.Persistence.InsertList(examples);

        var (response, output) =
            await fixture.ApiClient.Delete<object>(
                $"cast_members/{example.Id.ToString()}"
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        var castMemberExample = await fixture.Persistence.GetById(example.Id);
        castMemberExample.Should().BeNull();
    }

    [Fact(DisplayName = nameof(NotFound))]
    [Trait("EndToEnd/API", "CatMembers/Delete - EndPoints")]
    public async Task NotFound()
    {
        await fixture.Persistence.InsertList(
            fixture.GetExampleCastMembersList(5)
        );
        var randomGuid = Guid.NewGuid();

        var (response, output) =
            await fixture.ApiClient.Delete<ProblemDetails>(
                $"cast_members/{randomGuid.ToString()}"
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"CastMember '{randomGuid}' not found.");
    }

    public void Dispose() => fixture.CleanPersistence();
}