using FC.Codeflix.Catalog.Application.UseCases.Categories.CreateCategory;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{
    private readonly CreateCategoryTestFixture _fixture;
    public CreateCategoryTest(CreateCategoryTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async Task CreateCategory()
    {
        var repositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var input = _fixture.GetInput();
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

    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new CreateCategoryTestFixture();

        var invalidInputShortName = fixture.GetInput();
        invalidInputShortName.Name = invalidInputShortName.Name[..2];
        var inputLongName = fixture.GetInput();
        inputLongName.Name = new string('a', 256);

        var invalidInputNullDescription = fixture.GetInput();
        invalidInputNullDescription.Description = null!;
        var invalidInputTooLongDescription = fixture.GetInput();
        invalidInputTooLongDescription.Description = new string('a', 10_001);

        return
        [
            [invalidInputShortName, "Name should have at least 3 characters."],
            [inputLongName, "Name should be less or equal 255 characters."],
            [invalidInputNullDescription, "Description should not be null."],
            [invalidInputTooLongDescription, "Description should be less or equal 10000 characters."]
        ];
    }

    [Theory(DisplayName = nameof(ThrowsWenCantInstantiateAggregate))]
    [Trait("Application", "CreateCategory - Use Cases")]
    [MemberData(nameof(GetInvalidInputs))]
    public async Task ThrowsWenCantInstantiateAggregate(CreateCategoryInput input, string exceptionMessage)
    {
        var useCase = new CreateCategoryUseCase(
            _fixture.GetCategoryRepositoryMock().Object,
            _fixture.GetUnitOfWorkMock().Object
        );

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<EntityValidationException>()
            .WithMessage(exceptionMessage);
    }

    [Fact(DisplayName = nameof(CreateCategoryWithOnlyName))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async Task CreateCategoryWithOnlyName()
    {
        var repositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var input = new CreateCategoryInput(_fixture.GetValidCategoryName());

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
        var repositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var input = new CreateCategoryInput(
            _fixture.GetValidCategoryName(),
            _fixture.GetValidCategoryDescription());

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
