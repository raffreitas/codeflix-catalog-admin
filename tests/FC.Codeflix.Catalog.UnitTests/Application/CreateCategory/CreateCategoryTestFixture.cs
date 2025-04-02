using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Categories.CreateCategory;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.UnitTests.Common;
using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.CreateCategory;

[CollectionDefinition(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTestFixtureCollection : ICollectionFixture<CreateCategoryTestFixture>
{
}

public class CreateCategoryTestFixture : BaseFixture
{
    public string GetValidCategoryName()
    {
        var categoryName = string.Empty;
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if (categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10_000)
            categoryDescription = categoryDescription[..10_000];
        return categoryDescription;
    }

    public bool GetRandomBoolean()
        => Faker.Random.Bool();

    public CreateCategoryInput GetInput()
        => new(
            name: GetValidCategoryName(),
            description: GetValidCategoryDescription(),
            isActive: GetRandomBoolean()
        );

    public CreateCategoryInput GetInvalidInputShortName()
    {
        var input = GetInput();
        input.Name = input.Name[..2];
        return input;
    }

    public CreateCategoryInput GetInvalidInputTooLongName()
    {
        var input = GetInput();
        var tooLongNameForCategory = Faker.Commerce.ProductName();
        while (tooLongNameForCategory.Length <= 255)
            tooLongNameForCategory += Faker.Commerce.ProductName();
        input.Name = tooLongNameForCategory;
        return input;
    }

    public CreateCategoryInput GetInvalidInputDescriptionNull()
    {
        var input = GetInput();
        input.Description = null!;
        return input;
    }

    public CreateCategoryInput GetInvalidInputTooLongDescription()
    {
        var input = GetInput();
        var tooLongDescriptionForCategory = Faker.Commerce.ProductDescription();
        while (tooLongDescriptionForCategory.Length <= 10_000)
            tooLongDescriptionForCategory += Faker.Commerce.ProductDescription();
        input.Description = tooLongDescriptionForCategory;
        return input;
    }

    public Mock<ICategoryRepository> GetCategoryRepositoryMock() => new();
    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();
}
