using FC.Codeflix.Catalog.Domain.Enum;

using DomainEntity = FC.Codeflix.Catalog.Domain.Entities;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.Common;

public class CastMemberModelOutput(
    Guid id,
    string name,
    CastMemberType type,
    DateTime createdAt)
{
    public Guid Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public CastMemberType Type { get; private set; } = type;
    public DateTime CreatedAt { get; private set; } = createdAt;

    public static CastMemberModelOutput FromCastMember(DomainEntity.CastMember castMember)
        => new(
            castMember.Id,
            castMember.Name,
            castMember.Type,
            castMember.CreatedAt
        );

    public override string ToString()
    {
        return $"[Id] = {Id}, [Name] = {Name}, [CreatedAt] = {CreatedAt:HHmmfffffff}";
    }
}