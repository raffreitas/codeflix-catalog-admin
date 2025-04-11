namespace FC.Codeflix.Catalog.EndToEndTests.Api.Categories.CreateCategory;
public class CreateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new CreateCategoryTestFixture();
        var totalInvalidCases = 3;

        for (int index = 0; index < totalInvalidCases; index++)
        {
            var input = fixture.GetExampleInput();
            switch (index % totalInvalidCases)
            {
                case 0:
                    input.Name = fixture.GetInvalidNameShort();
                    yield return [input, "Name should have at least 3 characters."];
                    break;
                case 1:
                    input.Name = fixture.GetInvalidNameTooLong();
                    yield return [input, "Name should be less or equal 255 characters."];
                    break;
                case 3:
                    input.Description = null!;
                    yield return [input, "Description should not be null."];
                    break;
                default:
                    break;
            }
        }
    }
}