using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

using FluentAssertions;

using Moq;

using DomainEntity = FC.Codeflix.Catalog.Domain.Entities;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMembers.ListCastMembers;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMembers.ListCastMembers;

[Collection(nameof(ListCastMembersTestFixture))]
public class ListCastMembersTest
{
    private readonly ListCastMembersTestFixture _fixture;

    public ListCastMembersTest(ListCastMembersTestFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = nameof(List))]
    [Trait("Application", "ListCastMembers - Use Cases")]
    public async Task List()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var castMembersListExample = _fixture.GetExampleCastMembersList(3);
        var repositorySearchOutput = new SearchOutput<DomainEntity.CastMember>(
            1, 10, castMembersListExample.Count,
            (IReadOnlyList<DomainEntity.CastMember>)castMembersListExample
        );
        repositoryMock.Setup(x => x.Search(
            It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()
        )).ReturnsAsync(repositorySearchOutput);
        var input = new UseCase.ListCastMembersInput(1, 10, "", "", SearchOrder.Asc);
        var useCase = new UseCase.ListCastMembers(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(repositorySearchOutput.CurrentPage);
        output.PerPage.Should().Be(repositorySearchOutput.PerPage);
        output.Total.Should().Be(repositorySearchOutput.Total);
        output.Items.ToList().ForEach(outputItem =>
        {
            var example = castMembersListExample.Find(x => x.Id == outputItem.Id);
            example.Should().NotBeNull();
            outputItem.Name.Should().Be(example.Name);
            outputItem.Type.Should().Be(example.Type);
        });
        repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(x => (
                x.Page == input.Page
                && x.PerPage == input.PerPage
                && x.Search == input.Search
                && x.Order == input.Dir
                && x.OrderBy == input.Sort
            )),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact(DisplayName = nameof(RetursEmptyWhenIsEmpty))]
    [Trait("Application", "ListCastMembers - Use Cases")]
    public async Task RetursEmptyWhenIsEmpty()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var castMembersListExample = new List<DomainEntity.CastMember>();
        var repositorySearchOutput = new SearchOutput<DomainEntity.CastMember>(
            1, 10, castMembersListExample.Count,
            (IReadOnlyList<DomainEntity.CastMember>)castMembersListExample
        );
        repositoryMock.Setup(x => x.Search(
            It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()
        )).ReturnsAsync(repositorySearchOutput);
        var input = new UseCase.ListCastMembersInput(1, 10, "", "", SearchOrder.Asc);
        var useCase = new UseCase.ListCastMembers(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(repositorySearchOutput.CurrentPage);
        output.PerPage.Should().Be(repositorySearchOutput.PerPage);
        output.Total.Should().Be(repositorySearchOutput.Total);
        output.Items.Should().HaveCount(castMembersListExample.Count);
        repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(x => (
                x.Page == input.Page
                && x.PerPage == input.PerPage
                && x.Search == input.Search
                && x.Order == input.Dir
                && x.OrderBy == input.Sort
            )),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}