using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.EndToEndTests.Base;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Categories.Common;
public abstract class CategoryBaseFixture : BaseFixture
{
    public CategoryPersistence Persistence { get; set; }
    protected CategoryBaseFixture()
    {
        Persistence = new(CreateDbContext());
    }
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


    public string GetInvalidNameShort()
        => GetValidCategoryName()[..2];

    public string GetInvalidNameTooLong()
    {
        var tooLongNameForCategory = Faker.Commerce.ProductName();
        while (tooLongNameForCategory.Length <= 255)
            tooLongNameForCategory += Faker.Commerce.ProductName();
        return tooLongNameForCategory;
    }

    public string GetInvalidDescriptionTooLong()
    {
        var tooLongDescriptionForCategory = Faker.Commerce.ProductDescription();
        while (tooLongDescriptionForCategory.Length <= 10_000)
            tooLongDescriptionForCategory += Faker.Commerce.ProductDescription();
        return tooLongDescriptionForCategory;
    }

    public Category GetExampleCategory()
    {
        return new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());
    }

    public List<Category> GetExampleCategoriesList(int length = 10)
    {
        return [.. Enumerable.Range(1, length).Select(_ => GetExampleCategory())];
    }
}
