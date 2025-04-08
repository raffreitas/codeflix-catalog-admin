using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.IntegrationTests.Base;

using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository;

[CollectionDefinition(nameof(CategoryRepositoryTestFixture))]
public class CategoryRepositoryTestFixtureCollection : ICollectionFixture<CategoryRepositoryTestFixture>
{
}
public class CategoryRepositoryTestFixture : BaseFixture
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

    public Category GetExampleCategory()
      => new(
          GetValidCategoryName(),
          GetValidCategoryDescription(),
          GetRandomBoolean());

    public List<Category> GetExampleCategoryList(int length = 10)
      => [.. Enumerable.Range(0, length).Select(_ => GetExampleCategory())];

    public List<Category> GetExampleCategoryListWithNames(IEnumerable<string> names)
      => names.Select(name =>
      {
          var category = GetExampleCategory();
          category.Update(name);
          return category;
      }).ToList();

    public List<Category> CloneCategoriesListOrdered(List<Category> categoriesList, string orderBy, SearchOrder order)
    {
        var listClone = new List<Category>(categoriesList);
        return (orderBy.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => [.. listClone.OrderBy(x => x.Name)],
            ("name", SearchOrder.Desc) => [.. listClone.OrderByDescending(x => x.Name)],
            ("id", SearchOrder.Asc) => [.. listClone.OrderBy(x => x.Id)],
            ("id", SearchOrder.Desc) => [.. listClone.OrderByDescending(x => x.Id)],
            ("createdat", SearchOrder.Asc) => [.. listClone.OrderBy(x => x.CreatedAt)],
            ("createdat", SearchOrder.Desc) => [.. listClone.OrderByDescending(x => x.CreatedAt)],
            _ => [.. listClone.OrderBy(x => x.Name)],
        };
    }

    public CodeflixCatalogDbContext CreateDbContext(bool preserveData = false)
    {
        var dbContext = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                .UseInMemoryDatabase("integration-tests-db")
                .Options
        );

        if (preserveData == false)
            dbContext.Database.EnsureDeleted();

        return dbContext;
    }
}
