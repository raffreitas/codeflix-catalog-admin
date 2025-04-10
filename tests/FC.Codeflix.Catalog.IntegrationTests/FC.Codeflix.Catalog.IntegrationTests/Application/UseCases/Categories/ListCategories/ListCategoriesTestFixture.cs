﻿using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Categories.Common;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Categories.ListCategories;

[CollectionDefinition(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTestFixtureCollection : ICollectionFixture<ListCategoriesTestFixture>
{
}

public class ListCategoriesTestFixture : CategoryUseCasesBaseFixture
{
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
}
