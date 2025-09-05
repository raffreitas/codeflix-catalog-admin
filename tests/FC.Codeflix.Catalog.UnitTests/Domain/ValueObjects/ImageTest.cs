using FC.Codeflix.Catalog.Domain.ValueObjects;
using FC.Codeflix.Catalog.UnitTests.Common;

using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.ValueObjects;

[Trait("Domain", "Image - Value Object")]
public sealed class ImageTest : BaseFixture
{
    [Fact(DisplayName = nameof(Instantiate))]
    public void Instantiate()
    {
        var path = Faker.Image.PicsumUrl();

        var image = new Image(path);

        image.Path.Should().Be(path);
    }

    [Fact(DisplayName = nameof(EqualsByPath))]
    public void EqualsByPath()
    {
        var path = Faker.Image.PicsumUrl();

        var image = new Image(path);
        var sameImage = new Image(path);

        var itEquals = image == sameImage;
        itEquals.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(DifferentByPath))]
    public void DifferentByPath()
    {
        var path = Faker.Image.PicsumUrl();
        var differentPath = Faker.Image.PicsumUrl();

        var image = new Image(path);
        var differentImage = new Image(differentPath);

        var itDifferent = image != differentImage;
        itDifferent.Should().BeTrue();
    }
}