using Codeflix.Catalog.Application.UseCases.CastMembers.Common;

using Codeflix.Catalog.Domain.Enum;

using MediatR;

namespace Codeflix.Catalog.Application.UseCases.CastMembers.UpdateCastMember;

public class UpdateCastMemberInput(Guid id, string name, CastMemberType type) : IRequest<CastMemberModelOutput>
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public CastMemberType Type { get; set; } = type;
}