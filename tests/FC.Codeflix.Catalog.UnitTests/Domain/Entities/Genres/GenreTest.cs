using FC.Codeflix.Catalog.Domain.Entities;

using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entities.Genres;

[Collection(nameof(GenreTestFixture))]
public class GenreTest(GenreTestFixture fixture)
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Genre - Aggregates")]
    public void Instantiate()
    {
        var genreName = "Horror";
        var dateTimeBefore = DateTime.Now;

        var genre = new Genre(genreName);

        var dateTimeAfter = DateTime.Now.AddSeconds(1);
        genre.Should().NotBeNull();
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
        var genreName = "Horror";
        var dateTimeBefore = DateTime.Now;

        var genre = new Genre(genreName, isActive);

        var dateTimeAfter = DateTime.Now.AddSeconds(1);
        genre.Should().NotBeNull();
        genre.Name.Should().Be(genreName);
        genre.IsActive.Should().Be(isActive);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
        genre.CreatedAt.Should().BeOnOrAfter(dateTimeBefore);
        genre.CreatedAt.Should().BeOnOrBefore(dateTimeAfter);
    }
}
