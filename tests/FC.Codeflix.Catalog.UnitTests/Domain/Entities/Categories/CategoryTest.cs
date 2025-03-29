using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.Categories;

public class CategoryTest
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

        category.Should().NotBeNull();
        category.Name.Should().Be(validData.Name);
        category.Description.Should().Be(validData.Description);
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
        var validData = new
        {
            Name = "Category Name",
            Description = "Category Description",
        };
        var dateTimeBefore = DateTime.Now;

        var category = new Category(validData.Name, validData.Description, isActive);
        var dateTimeAfter = DateTime.Now;

        category.Should().NotBeNull();
        category.Name.Should().Be(validData.Name);
        category.Description.Should().Be(validData.Description);
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
        var action = () => new Category(name!, "Category Description");

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null.");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        var action = () => new Category("Category Name", null!);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Description should not be null.");
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThen3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("C")]
    [InlineData("Ca")]
    public void InstantiateErrorWhenNameIsLessThen3Characters(string invalidName)
    {
        var action = () => new Category(invalidName, "Category Description");

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should have at least 3 characters.");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThen255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameIsGreaterThen255Characters()
    {
        var invalidName = new string('A', 256);
        var action = () => new Category(invalidName, "Category Description");

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal 255 characters.");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThen10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsGreaterThen10_000Characters()
    {
        var invalidDescription = new string('A', 10_001);
        var action = () => new Category("Category Name", invalidDescription);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal 10000 characters.");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        var category = new Category("Category Name", "Category Description", isActive: false);

        category.Activate();

        category.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        var category = new Category("Category Name", "Category Description", isActive: true);

        category.Deactivate();

        category.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update()
    {
        var category = new Category("Category Name", "Category Description");
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
        var category = new Category("Category Name", "Category Description");
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
        var category = new Category("Category Name", "Category Description");

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
        var category = new Category("Category Name", "Category Description");

        var action = () => category.Update(invalidName!);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should have at least 3 characters.");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThen255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsGreaterThen255Characters()
    {
        var invalidName = new string('A', 256);
        var category = new Category("Category Name", "Category Description");

        var action = () => category.Update(invalidName!);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal 255 characters.");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionIsGreaterThen10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionIsGreaterThen10_000Characters()
    {
        var invalidDescription = new string('A', 10_001);
        var category = new Category("Category Name", "Category Description");

        var action = () => category.Update("Category Name", invalidDescription);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal 10000 characters.");
    }
}
