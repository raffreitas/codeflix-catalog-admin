namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Categories.UpdateCategory;
public class UpdateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetCategoriesToUpdate(int times = 10)
    {
        var fixture = new UpdateCategoryTestFixture();

        for (int index = 0; index < times; index++)
        {
            var exampleCategory = fixture.GetExampleCategory();
            var exampleInput = fixture.GetValidInput(exampleCategory.Id);

            yield return new object[]
            {
                exampleCategory,
                exampleInput
            };
        }
    }

    public static IEnumerable<object[]> GetInvalidInputs(int times = 12)
    {
        var fixture = new UpdateCategoryTestFixture();
        var totalInvalidCases = 3;

        for (int index = 0; index < times; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    yield return [fixture.GetInvalidInputShortName(), "Name should have at least 3 characters."];
                    break;
                case 1:
                    yield return [fixture.GetInvalidInputTooLongName(), "Name should be less or equal 255 characters."];
                    break;
                case 2:
                    yield return [fixture.GetInvalidInputTooLongDescription(), "Description should be less or equal 10000 characters."];
                    break;
                default:
                    break;
            }
        }
    }
}
