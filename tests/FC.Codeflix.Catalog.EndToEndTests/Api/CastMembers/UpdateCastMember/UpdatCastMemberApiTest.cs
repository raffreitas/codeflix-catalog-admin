using System.Net;

using FC.Codeflix.Catalog.Api.Models.CastMembers;
using FC.Codeflix.Catalog.Api.Models.Responses;
using FC.Codeflix.Catalog.Application.UseCases.CastMembers.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMembers.Common;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMembers.UpdateCastMember;

[Collection(nameof(CastMemberApiBaseFixture))]
public class UpdateCastMemberApiTest(CastMemberApiBaseFixture fixture) : IDisposable
{
    [Fact(DisplayName = nameof(Update))]
    [Trait("EndToEnd/API", "CastMembers/Update")]
    public async Task Update()
    {
        var examples = fixture.GetExampleCastMembersList(5);
        var example = examples[2];
        var newName = fixture.GetValidName();
        var newType = fixture.GetRandomCastMemberType();
        await fixture.Persistence.InsertList(examples);

        var (response, output) =
            await fixture.ApiClient.Put<ApiResponse<CastMemberModelOutput>>(
                $"cast_members/{example.Id.ToString()}",
                new UpdateCastMemberApiInput(newName, newType)
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(example.Id);
        output.Data.Name.Should().Be(newName);
        output.Data.Type.Should().Be(newType);
        var castMemberFromDb = await fixture.Persistence.GetById(example.Id);
        castMemberFromDb.Should().NotBeNull();
        castMemberFromDb.Id.Should().Be(example.Id);
        castMemberFromDb.Name.Should().Be(newName);
        castMemberFromDb.Type.Should().Be(newType);
    }

    [Fact(DisplayName = nameof(Retuns404IfNotFound))]
    [Trait("EndToEnd/API", "CastMembers/Update")]
    public async Task Retuns404IfNotFound()
    {
        var examples = fixture.GetExampleCastMembersList(5);
        var randomGuid = Guid.NewGuid();
        var newName = fixture.GetValidName();
        var newType = fixture.GetRandomCastMemberType();
        await fixture.Persistence.InsertList(examples);

        var (response, output) =
            await fixture.ApiClient.Put<ProblemDetails>(
                $"cast_members/{randomGuid}",
                new UpdateCastMemberApiInput(newName, newType)
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"CastMember '{randomGuid}' not found.");
    }

    [Fact(DisplayName = nameof(Retuns422IfThereAreValidationErrors))]
    [Trait("EndToEnd/API", "CastMembers/Update")]
    public async Task Retuns422IfThereAreValidationErrors()
    {
        var examples = fixture.GetExampleCastMembersList(5);
        var example = examples[2];
        const string newName = "";
        var newType = fixture.GetRandomCastMemberType();
        await fixture.Persistence.InsertList(examples);

        var (response, output) =
            await fixture.ApiClient.Put<ProblemDetails>(
                $"cast_members/{example.Id.ToString()}",
                new UpdateCastMemberApiInput(newName, newType)
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
        output.Should().NotBeNull();
        output.Title.Should().Be("One or more validation error occurred");
        output.Detail.Should().Be($"Name should not be null or empty.");
    }

    public void Dispose() => fixture.CleanPersistence();
}