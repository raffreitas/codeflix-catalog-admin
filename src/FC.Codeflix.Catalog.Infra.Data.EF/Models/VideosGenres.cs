using FC.Codeflix.Catalog.Domain.Entities;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Models;

public sealed class VideosGenres(Guid genreId, Guid videoId)
{
    public Guid GenreId { get; set; } = genreId;
    public Guid VideoId { get; set; } = videoId;
    public Genre? Genre { get; set; }
    public Video? Video { get; set; }
}