using System.Net;

using FC.Codeflix.Catalog.Api.Models.Responses;
using FC.Codeflix.Catalog.Application.UseCases.CastMembers.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMembers.Common;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMembers.GetCastMember;

[Collection(nameof(CastMemberApiBaseFixture))]
public class GetCastMemberApiTest(CastMemberApiBaseFixture fixture) : IDisposable
{
    [Fact(DisplayName = nameof(Get))]
    [Trait("EndToEnd/API", "CatMembers/Get - EndPoints")]
    public async Task Get()
    {
        var examples = fixture.GetExampleCastMembersList(5);
        var example = examples[2];
        await fixture.Persistence.InsertList(examples);

        var (response, output) =
            await fixture.ApiClient.Get<ApiResponse<CastMemberModelOutput>>(
                $"cast_members/{example.Id.ToString()}"
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(example.Id);
        output.Data.Name.Should().Be(example.Name);
        output.Data.Type.Should().Be(example.Type);
    }

    [Fact(DisplayName = nameof(NotFound))]
    [Trait("EndToEnd/API", "CatMembers/Get - EndPoints")]
    public async Task NotFound()
    {
        await fixture.Persistence.InsertList(
            fixture.GetExampleCastMembersList(5)
        );
        var randomGuid = Guid.NewGuid();

        var (response, output) =
            await fixture.ApiClient.Get<ProblemDetails>(
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