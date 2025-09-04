using FC.Codeflix.Catalog.Application.UseCases.CastMembers.Common;
using FC.Codeflix.Catalog.Domain.Enum;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.UpdateCastMember;

public class UpdateCastMemberInput : IRequest<CastMemberModelOutput>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public CastMemberType Type { get; set; }

    public UpdateCastMemberInput(Guid id, string name, CastMemberType type)
    {
        Id = id;
        Name = name;
        Type = type;
    }
}