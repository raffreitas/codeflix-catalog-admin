using Bogus;

using FC.Codeflix.Catalog.Application.UseCases.Categories.UpdateCategory;
using FC.Codeflix.Catalog.UnitTests.Application.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.Categories.UpdateCategory;

[CollectionDefinition(nameof(UpdateCategoryTestFixtureCollection))]
public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryTestFixture> { }

public class UpdateCategoryTestFixture : CategoryUseCasesBaseFixture
{
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
