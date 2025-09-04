using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.Api.Models.CastMembers;

public sealed record UpdateCastMemberApiInput(string Name, CastMemberType Type)
{
    public string Name { get; set; } = Name;
    public CastMemberType Type { get; set; } = Type;
}