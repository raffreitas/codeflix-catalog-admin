using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.Domain.Extensions;

public static class RatingExtensions
{
    public static Rating ToRating(this string? rating) => rating?.ToLower() switch
    {
        "er" => Rating.Er,
        "l" => Rating.L,
        "10" => Rating.Rate10,
        "12" => Rating.Rate12,
        "14" => Rating.Rate14,
        "16" => Rating.Rate16,
        "18" => Rating.Rate18,
        _ => throw new ArgumentOutOfRangeException(nameof(rating), $"Not expected rating value: {rating}"),
    };

    public static string ToStringSignal(this Rating rating) => rating switch
    {
        Rating.Er => "Er",
        Rating.L => "L",
        Rating.Rate10 => "10",
        Rating.Rate12 => "12",
        Rating.Rate14 => "14",
        Rating.Rate16 => "16",
        Rating.Rate18 => "18",
        _ => throw new ArgumentOutOfRangeException(nameof(rating), $"Not expected rating value: {rating}"),
    };
}