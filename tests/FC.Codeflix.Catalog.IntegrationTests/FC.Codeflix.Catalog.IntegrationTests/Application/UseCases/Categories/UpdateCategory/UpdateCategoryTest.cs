using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Categories.Common;
using FC.Codeflix.Catalog.Application.UseCases.Categories.UpdateCategory;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Categories.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest(UpdateCategoryTestFixture fixture)
{
    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    public async Task ThrowWhenCategoryNotFound()
    {
        var dbContext = fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var input = fixture.GetValidInput();
        var useCase = new UpdateCategoryUseCase(repository, unitOfWork);

        var act = async () => await useCase.Handle(input, CancellationToken.None);

        await act.Should()
            .ThrowAsync<NotFoundException>();
    }

    [Theory(DisplayName = nameof(UpdateCategory))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async Task UpdateCategory(Category exampleCategory, UpdateCategoryInput input)
    {
        var dbContext = fixture.CreateDbContext();
        await dbContext.AddRangeAsync(fixture.GetExampleCategoryList());
        var trackingInfo = await dbContext.AddAsync(exampleCategory);
        await dbContext.SaveChangesAsync();
        trackingInfo.State = EntityState.Detached;
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new UpdateCategoryUseCase(repository, unitOfWork);

        CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)input.IsActive!);
        var dbCategory = await fixture
           .CreateDbContext(preserveData: true)
           .Categories.SingleAsync((x) => x.Id == input.Id, CancellationToken.None);
        dbCategory.Should().NotBeNull();
        dbCategory.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be((bool)input.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
    }

    [Theory(DisplayName = nameof(UpdateCategoryOnlyName))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
    nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
    parameters: 5,
    MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async Task UpdateCategoryOnlyName(Category exampleCategory, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(
            exampleInput.Id,
            exampleInput.Name
        );

        var dbContext = fixture.CreateDbContext();
        await dbContext.AddRangeAsync(fixture.GetExampleCategoryList());
        var trackingInfo = await dbContext.AddAsync(exampleCategory);
        await dbContext.SaveChangesAsync();
        trackingInfo.State = EntityState.Detached;
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new UpdateCategoryUseCase(repository, unitOfWork);

        CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        var dbCategory = await fixture
           .CreateDbContext(preserveData: true)
           .Categories.SingleAsync((x) => x.Id == input.Id, CancellationToken.None);
        dbCategory.Should().NotBeNull();
        dbCategory.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
    }

    [Theory(DisplayName = nameof(UpdateCategoryWithoutProvidingIsActive))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async Task UpdateCategoryWithoutProvidingIsActive(Category exampleCategory, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(
            exampleInput.Id,
            exampleInput.Name,
            exampleInput.Description
        );

        var dbContext = fixture.CreateDbContext();
        await dbContext.AddRangeAsync(fixture.GetExampleCategoryList());
        var trackingInfo = await dbContext.AddAsync(exampleCategory);
        await dbContext.SaveChangesAsync();
        trackingInfo.State = EntityState.Detached;
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new UpdateCategoryUseCase(repository, unitOfWork);

        CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        var dbCategory = await fixture
           .CreateDbContext(preserveData: true)
           .Categories.SingleAsync((x) => x.Id == input.Id, CancellationToken.None);
        dbCategory.Should().NotBeNull();
        dbCategory.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
    }

    [Theory(DisplayName = nameof(ThrowWhenCantUpdateCategory))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 6,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async Task ThrowWhenCantUpdateCategory(UpdateCategoryInput input, string exceptionMessage)
    {
        var exampleCategory = fixture.GetExampleCategory();
        var dbContext = fixture.CreateDbContext();
        await dbContext.Categories.AddAsync(exampleCategory);
        await dbContext.SaveChangesAsync();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        input.Id = exampleCategory.Id;
        await dbContext.Categories.AddAsync(exampleCategory);

        var useCase = new UpdateCategoryUseCase(repository, unitOfWork);

        var act = async () => await useCase.Handle(input, CancellationToken.None);

        await act.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(exceptionMessage);
    }
}
