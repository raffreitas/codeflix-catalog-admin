using FC.Codeflix.Catalog.Domain.Entities;
using FC.Codeflix.Catalog.Domain.Exceptions;

using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.Genres;

[Collection(nameof(GenreTestFixture))]
public class GenreTest(GenreTestFixture fixture)
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Genre - Aggregates")]
    public void Instantiate()
    {
        var genreName = fixture.GetValidName();
        var dateTimeBefore = DateTime.Now;

        var genre = new Genre(genreName);

        var dateTimeAfter = DateTime.Now.AddSeconds(1);
        genre.Should().NotBeNull();
        genre.Id.Should().NotBeEmpty();
        genre.Name.Should().Be(genreName);
        genre.IsActive.Should().BeTrue();
        genre.CreatedAt.Should().NotBeSameDateAs(default);
        genre.CreatedAt.Should().BeOnOrAfter(dateTimeBefore);
        genre.CreatedAt.Should().BeOnOrBefore(dateTimeAfter);
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var genreName = fixture.GetValidName();
        var dateTimeBefore = DateTime.Now;

        var genre = new Genre(genreName, isActive);

        var dateTimeAfter = DateTime.Now.AddSeconds(1);
        genre.Should().NotBeNull();
        genre.Id.Should().NotBeEmpty();
        genre.Name.Should().Be(genreName);
        genre.IsActive.Should().Be(isActive);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
        genre.CreatedAt.Should().BeOnOrAfter(dateTimeBefore);
        genre.CreatedAt.Should().BeOnOrBefore(dateTimeAfter);
    }

    [Theory(DisplayName = nameof(Activate))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void Activate(bool isActive)
    {
        var genre = fixture.GetExampleGenre(isActive);

        genre.Activate();

        genre.Should().NotBeNull();
        genre.IsActive.Should().BeTrue();
        genre.CreatedAt.Should().NotBeSameDateAs(default);
    }


    [Theory(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void Deactivate(bool isActive)
    {
        var genre = fixture.GetExampleGenre(isActive);

        genre.Deactivate();

        genre.Should().NotBeNull();
        genre.IsActive.Should().BeFalse();
        genre.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Genre - Aggregates")]
    public void Update()
    {
        var genre = fixture.GetExampleGenre();
        var newName = fixture.GetValidName();

        genre.Update(newName);

        genre.Should().NotBeNull();
        genre.Name.Should().Be(newName);
        genre.IsActive.Should().Be(genre.IsActive);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(InstantiateThrowWhenNameEmpty))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void InstantiateThrowWhenNameEmpty(string? name)
    {
        var act = () => new Genre(name!);

        act.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be null or empty.");
    }

    [Theory(DisplayName = nameof(UpdateThrowWhenNameEmpty))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateThrowWhenNameEmpty(string? name)
    {
        var genre = fixture.GetExampleGenre();
        var act = () => genre.Update(name!);

        act.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be null or empty.");
    }

    [Fact(DisplayName = nameof(AddCategory))]
    [Trait("Domain", "Genre - Aggregates")]
    public void AddCategory()
    {
        var genre = fixture.GetExampleGenre();
        var categoryId = Guid.NewGuid();

        genre.AddCategory(categoryId);

        genre.Categories.Should().HaveCount(1);
        genre.Categories.Should().Contain(categoryId);
    }

    [Fact(DisplayName = nameof(AddTwoCategory))]
    [Trait("Domain", "Genre - Aggregates")]
    public void AddTwoCategory()
    {
        var genre = fixture.GetExampleGenre();
        var categoryId1 = Guid.NewGuid();
        var categoryId2 = Guid.NewGuid();

        genre.AddCategory(categoryId1);
        genre.AddCategory(categoryId2);

        genre.Categories.Should().HaveCount(2);
        genre.Categories.Should().Contain(categoryId1);
        genre.Categories.Should().Contain(categoryId2);
    }

    [Fact(DisplayName = nameof(RemoveCategory))]
    [Trait("Domain", "Genre - Aggregates")]
    public void RemoveCategory()
    {
        var exampleGuid = Guid.NewGuid();
        var genre = fixture.GetExampleGenre(
            categoriesIds: [Guid.NewGuid(), exampleGuid, Guid.NewGuid()]
        );

        genre.RemoveCategory(exampleGuid);

        genre.Categories.Should().HaveCount(2);
        genre.Categories.Should().NotContain(exampleGuid);
    }

    [Fact(DisplayName = nameof(RemoveAllCategories))]
    [Trait("Domain", "Genre - Aggregates")]
    public void RemoveAllCategories()
    {
        var genre = fixture.GetExampleGenre(
            categoriesIds: [Guid.NewGuid(), Guid.NewGuid()]
        );

        genre.RemoveAllCategories();

        genre.Categories.Should().BeEmpty();
    }
}
