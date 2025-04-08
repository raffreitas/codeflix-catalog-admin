using FC.Codeflix.Catalog.Application.UseCases.Categories.CreateCategory;
using FC.Codeflix.Catalog.UnitTests.Application.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.CreateCategory;

[CollectionDefinition(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTestFixtureCollection : ICollectionFixture<CreateCategoryTestFixture>
{
}

public class CreateCategoryTestFixture : CategoryUseCasesBaseFixture
{
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
}