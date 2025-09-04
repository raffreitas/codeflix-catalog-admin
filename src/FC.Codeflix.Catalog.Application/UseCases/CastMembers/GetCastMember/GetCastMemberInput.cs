using FC.Codeflix.Catalog.Application.UseCases.CastMembers.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.GetCastMember;
public class GetCastMemberInput
    : IRequest<CastMemberModelOutput>
{
    public Guid Id { get; private set; }
    public GetCastMemberInput(Guid id) => Id = id;
}
