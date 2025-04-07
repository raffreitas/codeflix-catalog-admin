using Bogus;

using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Categories.UpdateCategory;
using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Repositories;
using FC.Codeflix.Catalog.UnitTests.Common;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory;

[CollectionDefinition(nameof(UpdateCategoryTestFixtureCollection))]
public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryTestFixture> { }

public class UpdateCategoryTestFixture : BaseFixture
{
    public Mock<ICategoryRepository> GetCategoryRepositoryMock() => new();
    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();
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

    public Category GetValidCategory()
        => new(GetValidCategoryName(), GetValidCategoryDescription());

    public bool GetRandomBoolean()
        => Faker.Random.Bool();

    public Category GetExampleCategory()
        => new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean());

    public UpdateCategoryInput GetValidInput(Guid? id = null)
        => new(
            id ?? Guid.NewGuid(),
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean());

    public UpdateCategoryInput GetInvalidInputShortName()
    {
        var input = GetValidInput();
        input.Name = input.Name[..2];
        return input;
    }

    public UpdateCategoryInput GetInvalidInputTooLongName()
    {
        var input = GetValidInput();
        var tooLongNameForCategory = Faker.Commerce.ProductName();
        while (tooLongNameForCategory.Length <= 255)
            tooLongNameForCategory += Faker.Commerce.ProductName();
        input.Name = tooLongNameForCategory;
        return input;
    }

    public UpdateCategoryInput GetInvalidInputTooLongDescription()
    {
        var input = GetValidInput();
        var tooLongDescriptionForCategory = Faker.Commerce.ProductDescription();
        while (tooLongDescriptionForCategory.Length <= 10_000)
            tooLongDescriptionForCategory += Faker.Commerce.ProductDescription();
        input.Description = tooLongDescriptionForCategory;
        return input;
    }
}
