using FC.Codeflix.Catalog.Application.UseCases.CastMembers.Common;
using FC.Codeflix.Catalog.Domain.Enum;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.CreateCastMember;
public class CreateCastMemberInput
    : IRequest<CastMemberModelOutput>
{
    public string Name { get; private set; }
    public CastMemberType Type { get; private set; }

    public CreateCastMemberInput(string name, CastMemberType type)
    {
        Name = name;
        Type = type;
    }
}
