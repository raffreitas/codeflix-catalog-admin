using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repositories;

using FluentAssertions;

using Moq;

using DomainEntity = FC.Codeflix.Catalog.Domain.Entities;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMembers.CreateCastMember;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMembers.CreateCastMember;

[Collection(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTest
{
    private readonly CreateCastMemberTestFixture _fixture;

    public CreateCastMemberTest(CreateCastMemberTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Create))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    public async Task Create()
    {
        var input = new UseCase.CreateCastMemberInput(
            _fixture.GetValidName(),
            _fixture.GetRandomCastMemberType()
        );
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateCastMember(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Type.Should().Be(input.Type);
        output.CreatedAt.Should().NotBeSameDateAs(default);
        unitOfWorkMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );
        repositoryMock.Verify(
            x => x.Insert(
                It.Is<DomainEntity.CastMember>(x => (x.Name == input.Name && x.Type == input.Type)
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }

    [Theory(DisplayName = nameof(ThrowsWhenInvalidName))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public async Task ThrowsWhenInvalidName(string? name)
    {
        var input = new UseCase.CreateCastMemberInput(
            name!,
            _fixture.GetRandomCastMemberType()
        );
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateCastMember(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var action = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<EntityValidationException>()
            .WithMessage("Name should not be null or empty.");
    }
}