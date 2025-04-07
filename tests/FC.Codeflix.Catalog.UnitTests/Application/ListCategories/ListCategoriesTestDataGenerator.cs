using FC.Codeflix.Catalog.Application.UseCases.Categories.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearcheableRepository;

namespace FC.Codeflix.Catalog.UnitTests.Application.ListCategories;
public class ListCategoriesTestDataGenerator
{
    public static IEnumerable<object[]> GetInputsWithoutAllParameters(int times = 14)
    {
        var fixture = new ListCategoriesTestFixture();
        var inputExample = fixture.GetExampleInput();
        for (int index = 0; index < times; index++)
        {
            switch (index % 7)
            {
                case 0:
                    yield return new object[]
                    {
                        new ListCategoriesInput()
                    };
                    break;
                case 1:
                    yield return new object[]
                    {
                        new ListCategoriesInput(inputExample.Page)
                    };
                    break;
                case 2:
                    yield return new object[]
                    {
                        new ListCategoriesInput(
                           inputExample.Page,
                           inputExample.PerPage
                        )
                    };
                    break;
                case 3:
                    yield return new object[]
                    {
                        new ListCategoriesInput(
                            inputExample.Page,
                            inputExample.PerPage,
                            inputExample.Search
                        )
                    };
                    break;
                case 4:
                    yield return new object[]
                    {
                        new ListCategoriesInput(
                            inputExample.Page,
                            inputExample.PerPage,
                            inputExample.Search,
                            inputExample.Sort
                        )
                    };
                    break;
                case 5:
                    yield return new object[]
                    {
                        new ListCategoriesInput(
                            page: inputExample.Page,
                            perPage: inputExample.PerPage,
                            search: inputExample.Search,
                            sort: inputExample.Sort,
                            dir: SearchOrder.Desc
                        )
                    };
                    break;
                case 6:
                    yield return new object[] { inputExample }; 
                    break;
                default:
                    break;
            }
        }
    }
}
