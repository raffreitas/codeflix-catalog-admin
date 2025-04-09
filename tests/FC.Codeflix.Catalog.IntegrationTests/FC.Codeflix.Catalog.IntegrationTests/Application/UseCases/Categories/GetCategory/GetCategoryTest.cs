using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Categories.GetCategory;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

using FluentAssertions;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Categories.GetCategory;

[Collection(nameof(GetCategoryTestFixture))]
public class GetCategoryTest(GetCategoryTestFixture fixture)
{
    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("Integration/Application", "GetCategory - Use Cases")]
    public async Task GetCategory()
    {
        var exampleCategory = fixture.GetExampleCategory();
        var dbContext = fixture.CreateDbContext();
        var categoryRepository = new CategoryRepository(dbContext);
        await dbContext.Categories.AddAsync(exampleCategory);
        await dbContext.SaveChangesAsync();
        var input = new GetCategoryInput(exampleCategory.Id);
        var useCase = new GetCategoryUseCase(categoryRepository);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(exampleCategory.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(NotFoundExceptionWhenCategoryDoesntExists))]
    [Trait("Application", "GetCategory - Use Cases")]
    public async Task NotFoundExceptionWhenCategoryDoesntExists()
    {
        var exampleGuid = Guid.NewGuid();
        var exampleCategory = fixture.GetExampleCategory();
        var dbContext = fixture.CreateDbContext();
        await dbContext.Categories.AddAsync(exampleCategory);
        await dbContext.SaveChangesAsync();

        var repository = new CategoryRepository(dbContext);

        var input = new GetCategoryInput(exampleGuid);
        var useCase = new GetCategoryUseCase(repository);

        var act = async () => await useCase.Handle(input, CancellationToken.None);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Category '{exampleGuid}' not found.");
    }
}
