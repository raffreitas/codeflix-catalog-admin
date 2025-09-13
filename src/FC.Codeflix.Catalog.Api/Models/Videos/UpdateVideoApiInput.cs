using FC.Codeflix.Catalog.Application.UseCases.Videos.UpdateVideo;
using FC.Codeflix.Catalog.Domain.Extensions;

namespace FC.Codeflix.Catalog.Api.Models.Videos;

public sealed record UpdateVideoApiInput
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int YearLaunched { get; set; }
    public bool Opened { get; set; }
    public bool Published { get; set; }
    public int Duration { get; set; }
    public string? Rating { get; set; }
    public List<Guid>? CategoriesIds { get; set; }
    public List<Guid>? GenresIds { get; set; }
    public List<Guid>? CastMembersIds { get; set; }

    public UpdateVideoInput ToInput(Guid id) => new(
        id,
        Title,
        Description,
        YearLaunched,
        Opened,
        Published,
        Duration,
        Rating.ToRating(),
        GenresIds,
        CategoriesIds,
        CastMembersIds
    );
}