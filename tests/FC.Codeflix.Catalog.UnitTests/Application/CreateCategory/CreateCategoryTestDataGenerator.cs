namespace FC.Codeflix.Catalog.UnitTests.Application.CreateCategory;
public class CreateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs(int times = 12)
    {
        var fixture = new CreateCategoryTestFixture();
        var totalInvalidCases = 4;

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
                    yield return [fixture.GetInvalidInputDescriptionNull(), "Description should not be null."];
                    break;
                case 3:
                    yield return [fixture.GetInvalidInputTooLongDescription(), "Description should be less or equal 10000 characters."];
                    break;
                default:
                    break;
            }
        }
    }
}
