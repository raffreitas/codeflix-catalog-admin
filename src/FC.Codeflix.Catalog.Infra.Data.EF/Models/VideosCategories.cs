using FC.Codeflix.Catalog.Domain.Entities;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Models;

public sealed class VideosCategories(Guid categoryId, Guid videoId)
{
    public Guid CategoryId { get; set; } = categoryId;
    public Guid VideoId { get; set; } = videoId;
    public Category? Category { get; set; }
    public Video? Video { get; set; }
}