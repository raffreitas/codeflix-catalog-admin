using FC.Codeflix.Catalog.Application.UseCases.CastMembers.Common;
using FC.Codeflix.Catalog.Domain.Enum;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.CreateCastMember;
public class CreateCastMemberInput(string name, CastMemberType type) : IRequest<CastMemberModelOutput>
{
    public string Name { get; private set; } = name;
    public CastMemberType Type { get; private set; } = type;
}
