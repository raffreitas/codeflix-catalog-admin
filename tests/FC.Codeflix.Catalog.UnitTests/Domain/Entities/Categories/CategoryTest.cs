using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.Categories;

[Collection(nameof(CategoryTestFixture))]
public class CategoryTest
{
    private readonly CategoryTestFixture _categoryTestFixture;

    public CategoryTest(CategoryTestFixture categoryTestFixture)
        => _categoryTestFixture = categoryTestFixture;

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var dateTimeBefore = DateTime.Now;

        var category = new Category(validCategory.Name, validCategory.Description);
        var dateTimeAfter = DateTime.Now;

        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default);
        category.CreatedAt.Should().BeAfter(dateTimeBefore);
        category.CreatedAt.Should().BeBefore(dateTimeAfter);
        category.IsActive.Should().BeTrue();
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var dateTimeBefore = DateTime.Now;

        var category = new Category(validCategory.Name, validCategory.Description, isActive);
        var dateTimeAfter = DateTime.Now;

        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default);
        category.CreatedAt.Should().BeAfter(dateTimeBefore);
        category.CreatedAt.Should().BeBefore(dateTimeAfter);
        category.IsActive.Should().Be(isActive);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void InstantiateErrorWhenNameIsEmpty(string? name)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var action = () => new Category(name!, validCategory.Description);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null.");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var action = () => new Category(validCategory.Name, null!);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Description should not be null.");
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThen3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("C")]
    [InlineData("Ca")]
    public void InstantiateErrorWhenNameIsLessThen3Characters(string invalidName)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var action = () => new Category(invalidName, validCategory.Description);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should have at least 3 characters.");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThen255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameIsGreaterThen255Characters()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var invalidName = new string('A', 256);

        var action = () => new Category(invalidName, validCategory.Description);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal 255 characters.");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThen10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsGreaterThen10_000Characters()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var invalidDescription = new string('A', 10_001);

        var action = () => new Category(validCategory.Name, invalidDescription);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal 10000 characters.");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var category = new Category(validCategory.Name, validCategory.Description, isActive: false);

        category.Activate();

        category.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var category = new Category(validCategory.Name, validCategory.Description, isActive: true);

        category.Deactivate();

        category.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var newValues = new
        {
            Name = "New Category Name",
            Description = "New Category Description"
        };

        category.Update(newValues.Name, newValues.Description);

        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(newValues.Description);
    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateOnlyName()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var newValues = new { Name = "New Category Name" };
        var currentDescription = category.Description;

        category.Update(newValues.Name);

        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(currentDescription);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void UpdateErrorWhenNameIsEmpty(string? name)
    {
        var category = _categoryTestFixture.GetValidCategory();

        var action = () => category.Update(name!);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null.");
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThen3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("C")]
    [InlineData("Ca")]
    public void UpdateErrorWhenNameIsLessThen3Characters(string invalidName)
    {
        var category = _categoryTestFixture.GetValidCategory();

        var action = () => category.Update(invalidName!);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should have at least 3 characters.");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThen255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsGreaterThen255Characters()
    {
        var invalidName = new string('A', 256);
        var category = _categoryTestFixture.GetValidCategory();

        var action = () => category.Update(invalidName!);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal 255 characters.");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionIsGreaterThen10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionIsGreaterThen10_000Characters()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var invalidDescription = new string('A', 10_001);

        var action = () => category.Update(category.Name, invalidDescription);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal 10000 characters.");
    }
}
