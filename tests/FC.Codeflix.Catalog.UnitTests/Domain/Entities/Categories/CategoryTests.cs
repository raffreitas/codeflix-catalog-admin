using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.Categories;

public class CategoryTests
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        var validData = new
        {
            Name = "Category Name",
            Description = "Category Description",
        };
        var dateTimeBefore = DateTime.Now;

        var category = new Category(validData.Name, validData.Description);
        var dateTimeAfter = DateTime.Now;

        Assert.NotNull(category);
        Assert.Equal(validData.Name, category.Name);
        Assert.Equal(validData.Description, category.Description);
        Assert.NotEqual(default, category.Id);
        Assert.NotEqual(default, category.CreatedAt);
        Assert.True(category.CreatedAt > dateTimeBefore);
        Assert.True(category.CreatedAt < dateTimeAfter);
        Assert.True(category.IsActive);
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var validData = new
        {
            Name = "Category Name",
            Description = "Category Description",
        };
        var dateTimeBefore = DateTime.Now;

        var category = new Category(validData.Name, validData.Description, isActive);
        var dateTimeAfter = DateTime.Now;

        Assert.NotNull(category);
        Assert.Equal(validData.Name, category.Name);
        Assert.Equal(validData.Description, category.Description);
        Assert.NotEqual(default, category.Id);
        Assert.NotEqual(default, category.CreatedAt);
        Assert.True(category.CreatedAt > dateTimeBefore);
        Assert.True(category.CreatedAt < dateTimeAfter);
        Assert.Equal(isActive, category.IsActive);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void InstantiateErrorWhenNameIsEmpty(string? name)
    {
        var action = () => new Category(name!, "Category Description");

        var exception = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Name should not be empty or null.", exception.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        var action = () => new Category("Category Name", null!);

        var exception = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Description should not be null.", exception.Message);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThen3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("C")]
    [InlineData("Ca")]
    public void InstantiateErrorWhenNameIsLessThen3Characters(string invalidName)
    {
        var action = () => new Category(invalidName, "Category Description");

        var exception = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Name should have at least 3 characters.", exception.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThen255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameIsGreaterThen255Characters()
    {
        var invalidName = new string('A', 256);
        var action = () => new Category(invalidName, "Category Description");

        var exception = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Name should be less or equal 255 characters.", exception.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThen10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsGreaterThen10_000Characters()
    {
        var invalidDescription = new string('A', 10_001);
        var action = () => new Category("Category Name", invalidDescription);

        var exception = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Description should be less or equal 10000 characters.", exception.Message);
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        var category = new Category("Category Name", "Category Description", isActive: false);

        category.Activate();

        Assert.True(category.IsActive);
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        var category = new Category("Category Name", "Category Description", isActive: true);

        category.Deactivate();

        Assert.False(category.IsActive);
    }
}
