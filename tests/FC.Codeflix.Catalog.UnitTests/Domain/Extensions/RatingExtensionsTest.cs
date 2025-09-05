using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Extensions;

using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Extensions;

[Trait("Domain", "Rating - Extensions")]
public sealed class RatingExtensionsTest
{
    [Theory(DisplayName = nameof(StringToRating))]
    [InlineData("Er", Rating.Er)]
    [InlineData("L", Rating.L)]
    [InlineData("10", Rating.Rate10)]
    [InlineData("12", Rating.Rate12)]
    [InlineData("14", Rating.Rate14)]
    [InlineData("16", Rating.Rate16)]
    [InlineData("18", Rating.Rate18)]
    public void StringToRating(string enumRating, Rating expectedRating)
    {
        var actualRating = enumRating.ToRating();
        actualRating.Should().Be(expectedRating);
    }

    [Fact(DisplayName = nameof(ThrowsExceptionWhenRatingIsInvalid))]
    public void ThrowsExceptionWhenRatingIsInvalid()
    {
        Action action = () => "invalid_rating".ToRating();
        action.Should()
            .Throw<ArgumentOutOfRangeException>()
            .WithMessage("Not expected rating value: invalid_rating*")
            .Where(e => e.ParamName == "rating");
    }

    [Theory(DisplayName = nameof(RatingToString))]
    [InlineData(Rating.Er, "Er")]
    [InlineData(Rating.L, "L")]
    [InlineData(Rating.Rate10, "10")]
    [InlineData(Rating.Rate12, "12")]
    [InlineData(Rating.Rate14, "14")]
    [InlineData(Rating.Rate16, "16")]
    [InlineData(Rating.Rate18, "18")]
    public void RatingToString(Rating rating, string expectedString)
    {
        var actualRating = rating.ToStringSignal();
        actualRating.Should().Be(expectedString);
    }
}