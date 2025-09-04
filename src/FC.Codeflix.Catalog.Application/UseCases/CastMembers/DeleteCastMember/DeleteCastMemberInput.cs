using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.DeleteCastMember;
public class DeleteCastMemberInput
    : IRequest
{
    public Guid Id { get; private set; }
    public DeleteCastMemberInput(Guid id) => Id = id;
}
