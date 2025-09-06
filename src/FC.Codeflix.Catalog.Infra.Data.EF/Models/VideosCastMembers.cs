using FC.Codeflix.Catalog.Domain.Entities;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Models;

public sealed class VideosCastMembers(Guid castMemberId, Guid videoId)
{
    public Guid CastMemberId { get; set; } = castMemberId;
    public Guid VideoId { get; set; } = videoId;
    public CastMember? CastMember { get; set; }
    public Video? Video { get; set; }
}