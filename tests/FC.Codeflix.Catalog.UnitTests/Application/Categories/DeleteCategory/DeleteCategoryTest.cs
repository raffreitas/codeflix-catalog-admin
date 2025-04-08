using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Categories.DeleteCategory;

using FluentAssertions;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Categories.DeleteCategory;

[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTest
{
    private readonly DeleteCategoryTestFixture _fixture;

    public DeleteCategoryTest(DeleteCategoryTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("Application", "DeleteCategory - Use Cases")]
    public async Task DeleteCategory()
    {
        var categoryExample = _fixture.GetExampleCategory();
        var repositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        repositoryMock.Setup(repositoryMock => repositoryMock
            .Get(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(categoryExample);

        var input = new DeleteCategoryInput(categoryExample.Id);
        var useCase = new DeleteCategoryUseCase(repositoryMock.Object, unitOfWorkMock.Object);

        await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            x => x.Get(categoryExample.Id, It.IsAny<CancellationToken>()),
            Times.Once
        );
        repositoryMock.Verify(
            x => x.Delete(categoryExample, It.IsAny<CancellationToken>()),
            Times.Once
        );
        unitOfWorkMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Application", "DeleteCategory - Use Cases")]
    public async Task ThrowWhenCategoryNotFound()
    {
        var exampleGuid = Guid.Empty;
        var repositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        repositoryMock.Setup(repositoryMock => repositoryMock
            .Get(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new NotFoundException($"Category '{exampleGuid}' not found."));

        var input = new DeleteCategoryInput(exampleGuid);
        var useCase = new DeleteCategoryUseCase(repositoryMock.Object, unitOfWorkMock.Object);

        var act = async () => await useCase.Handle(input, CancellationToken.None);

        await act.Should()
            .ThrowAsync<NotFoundException>();

        repositoryMock.Verify(
            x => x.Get(exampleGuid, It.IsAny<CancellationToken>()),
            Times.Once
        );
        unitOfWorkMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}