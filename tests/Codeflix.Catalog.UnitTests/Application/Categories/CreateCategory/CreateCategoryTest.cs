using Codeflix.Catalog.Application.UseCases.Categories.CreateCategory;
using Codeflix.Catalog.Domain.Entities;
using Codeflix.Catalog.Domain.Exceptions;

using FluentAssertions;

using Moq;

namespace Codeflix.Catalog.UnitTests.Application.Categories.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest(CreateCategoryTestFixture fixture)
{
    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async Task CreateCategory()
    {
        var repositoryMock = fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();
        var input = fixture.GetInput();
        var useCase = new CreateCategoryUseCase(repositoryMock.Object, unitOfWorkMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            x => x.Insert(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        unitOfWorkMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(ThrowsWenCantInstantiateCategory))]
    [Trait("Application", "CreateCategory - Use Cases")]
    [MemberData(
        nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 24,
        MemberType = typeof(CreateCategoryTestDataGenerator)
    )]
    public async Task ThrowsWenCantInstantiateCategory(CreateCategoryInput input, string exceptionMessage)
    {
        var useCase = new CreateCategoryUseCase(
            fixture.GetCategoryRepositoryMock().Object,
            fixture.GetUnitOfWorkMock().Object
        );

        Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

        await act.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(exceptionMessage);
    }

    [Fact(DisplayName = nameof(CreateCategoryWithOnlyName))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async Task CreateCategoryWithOnlyName()
    {
        var repositoryMock = fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();
        var input = new CreateCategoryInput(fixture.GetValidCategoryName());

        var useCase = new CreateCategoryUseCase(repositoryMock.Object, unitOfWorkMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            x => x.Insert(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        unitOfWorkMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Description.Should().BeEmpty();
        output.IsActive.Should().BeTrue();
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(CreateCategoryWithOnlyNameAndDescription))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async Task CreateCategoryWithOnlyNameAndDescription()
    {
        var repositoryMock = fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();
        var input = new CreateCategoryInput(
            fixture.GetValidCategoryName(),
            fixture.GetValidCategoryDescription());

        var useCase = new CreateCategoryUseCase(repositoryMock.Object, unitOfWorkMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            x => x.Insert(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        unitOfWorkMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().BeTrue();
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }
}