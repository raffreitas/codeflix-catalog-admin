using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repositories;

using FluentAssertions;

using Moq;

using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMembers.DeleteCastMember;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entities;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMembers.DeleteCastMember;

[Collection(nameof(DeleteCastMemberFixture))]
public class DeleteCastMemberTest
{
    private readonly DeleteCastMemberFixture _fixture;

    public DeleteCastMemberTest(DeleteCastMemberFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(DeleteCastMember))]
    [Trait("Application", "DeleteCastMember - Use Cases")]
    public async Task DeleteCastMember()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var castMemberExample = _fixture.GetExampleCastMember();
        repositoryMock
            .Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(castMemberExample);
        var input = new UseCase.DeleteCastMemberInput(castMemberExample.Id);
        var useCase = new UseCase.DeleteCastMember(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().NotThrowAsync();
        repositoryMock.Verify(
            x => x.Get(It.Is<Guid>(x => x == input.Id), It.IsAny<CancellationToken>()),
            Times.Once
        );
        repositoryMock.Verify(
            x => x.Delete(
                It.Is<DomainEntity.CastMember>(x => x.Id == input.Id),
                It.IsAny<CancellationToken>()),
            Times.Once
        );
        unitOfWorkMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>())
            , Times.Once
        );
    }

    [Fact(DisplayName = nameof(ThrowsWhenNotFound))]
    [Trait("Application", "DeleteCastMember - Use Cases")]
    public async Task ThrowsWhenNotFound()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        repositoryMock
            .Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("notFound"));
        var input = new UseCase.DeleteCastMemberInput(Guid.NewGuid());
        var useCase = new UseCase.DeleteCastMember(
            repositoryMock.Object,
            Mock.Of<IUnitOfWork>()
        );

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
    }
}