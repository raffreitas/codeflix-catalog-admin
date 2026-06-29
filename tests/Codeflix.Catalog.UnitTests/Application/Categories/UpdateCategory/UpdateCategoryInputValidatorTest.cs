using Codeflix.Catalog.Application.UseCases.Categories.UpdateCategory;

using FluentAssertions;

namespace Codeflix.Catalog.UnitTests.Application.Categories.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixtureCollection))]
public class UpdateCategoryInputValidatorTest(UpdateCategoryTestFixture fixture)
{
    [Fact(DisplayName = nameof(DontValidateWhenEmptyGuid))]
    [Trait("Application", "UpdateCategoryInputValidator - Use Cases")]
    public void DontValidateWhenEmptyGuid()
    {
        var input = fixture.GetValidInput(Guid.Empty);
        var validator = new UpdateCategoryInputValidator();

        var result = validator.Validate(input);

        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be("'Id' must not be empty.");
    }

    [Fact(DisplayName = nameof(ValidateWhenValid))]
    [Trait("Application", "UpdateCategoryInputValidator - Use Cases")]
    public void ValidateWhenValid()
    {
        var input = fixture.GetValidInput();
        var validator = new UpdateCategoryInputValidator();

        var result = validator.Validate(input);

        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().HaveCount(0);
    }
}
