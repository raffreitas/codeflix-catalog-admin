using System.Net;

using Codeflix.Catalog.EndToEndTests.Api.CastMembers.Common;
using Codeflix.Catalog.EndToEndTests.Models;
using Codeflix.Catalog.Application.UseCases.CastMembers.Common;
using Codeflix.Catalog.Application.UseCases.CastMembers.CreateCastMember;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codeflix.Catalog.EndToEndTests.Api.CastMembers.CreateCastMember;

[Collection(nameof(CastMemberApiBaseFixture))]
public class CreateCastMemberApiTest(CastMemberApiBaseFixture fixture) : IDisposable
{
    [Fact(DisplayName = nameof(Create))]
    [Trait("End2End/Api", "CastMembers/Create")]
    public async Task Create()
    {
        var example = fixture.GetExampleCastMember();

        var (response, output) =
            await fixture.ApiClient.Post<TestApiResponse<CastMemberModelOutput>>(
                "/cast_members",
                new CreateCastMemberInput(example.Name, example.Type)
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
        output.Should().NotBeNull();
        output.Data!.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(example.Name);
        output.Data.Type.Should().Be(example.Type);
        var castMemberInDb = await fixture.Persistence.GetById(output.Data.Id);
        castMemberInDb.Should().NotBeNull();
        castMemberInDb.Name.Should().Be(example.Name);
        castMemberInDb.Type.Should().Be(example.Type);
    }

    [Fact(DisplayName = nameof(ThrowWhenNameIsEmpty))]
    [Trait("End2End/Api", "CastMembers/Create")]
    public async Task ThrowWhenNameIsEmpty()
    {
        var example = fixture.GetExampleCastMember();

        var (response, output) =
            await fixture.ApiClient.Post<ProblemDetails>(
                "/cast_members",
                new CreateCastMemberInput("", example.Type)
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
        output.Should().NotBeNull();
        output.Title.Should().Be("One or more validation error occurred");
        output.Detail.Should().Be("Name should not be null or empty.");
    }

    public void Dispose() => fixture.CleanPersistence();
}