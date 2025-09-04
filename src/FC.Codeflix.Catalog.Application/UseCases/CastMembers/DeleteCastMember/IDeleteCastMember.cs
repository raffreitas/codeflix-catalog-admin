using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMembers.DeleteCastMember;
public interface IDeleteCastMember
    : IRequestHandler<DeleteCastMemberInput>
{ }
