using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Exceptions;

using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.CastMembers;

[Collection(nameof(CastMemberTestFixture))]
public class CastMemberTest(CastMemberTestFixture fixture)
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "CastMember - Aggregates")]
    public void Instantiate()
    {
        var datetimeBefore = DateTime.Now.AddSeconds(-1);
        var name = fixture.GetValidName();
        var type = fixture.GetRandomCastMemberType();

        var castMember = new CastMember(name, type);

        var datetimeAfter = DateTime.Now.AddSeconds(1);
        castMember.Id.Should().NotBe(Guid.Empty);
        castMember.Name.Should().Be(name);
        castMember.Type.Should().Be(type);
        (castMember.CreatedAt >= datetimeBefore).Should().BeTrue();
        (castMember.CreatedAt <= datetimeAfter).Should().BeTrue();
    }

    [Theory(DisplayName = nameof(ThrowErrorWhenNameIsInvalid))]
    [Trait("Domain", "CastMember - Aggregates")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ThrowErrorWhenNameIsInvalid(string? name)
    {
        var type = fixture.GetRandomCastMemberType();

        var action = () => new CastMember(name!, type);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be null or empty.");
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "CastMember - Aggregates")]
    public void Update()
    {
        var newName = fixture.GetValidName();
        var newType = fixture.GetRandomCastMemberType();
        var castMember = fixture.GetExampleCastMember();

        castMember.Update(newName, newType);

        castMember.Name.Should().Be(newName);
        castMember.Type.Should().Be(newType);
    }

    [Theory(DisplayName = nameof(UpdateThrowsErrorWhenNameIsInvalid))]
    [Trait("Domain", "CastMember - Aggregates")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateThrowsErrorWhenNameIsInvalid(string? newName)
    {
        var newType = fixture.GetRandomCastMemberType();
        var castMember = fixture.GetExampleCastMember();

        var action = () => castMember.Update(newName!, newType);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be null or empty.");
    }
}